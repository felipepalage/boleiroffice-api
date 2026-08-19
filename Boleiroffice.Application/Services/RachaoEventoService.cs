using System.Globalization;
using System.Text;
using Boleiroffice.Application.Common.Validation;
using Boleiroffice.Application.DTOs.Rachao;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class RachaoEventoService : IRachaoEventoService
{
    private readonly IRachaoEventoRepository _repo;
    private readonly IAmistosoRepository _amistosoRepo;
    private readonly INotificationService _notifier;

    public RachaoEventoService(IRachaoEventoRepository repo, IAmistosoRepository amistosoRepo, INotificationService notifier)
    {
        _repo = repo;
        _amistosoRepo = amistosoRepo;
        _notifier = notifier;
    }

    public async Task<RachaoEventoResponse> CriarAsync(Guid empresaId, CriarRachaoRequest request, CancellationToken cancellationToken)
    {
        var evento = new RachaoEvento
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Token = Guid.NewGuid().ToString("N")[..10],
            HorarioEvento = DateTime.SpecifyKind(request.HorarioEvento, DateTimeKind.Utc),
            JogadoresPorTime = request.JogadoresPorTime,
            NumeroTimes = 0,
            SorteioFeito = false,
            DataCriacao = DateTime.UtcNow
        };

        await _repo.AddAsync(evento, cancellationToken);
        return MapEvento(evento);
    }

    public async Task<RachaoEventoResponse?> GetAtivoAsync(Guid empresaId, CancellationToken cancellationToken)
    {
        var evento = await _repo.GetAtivoByEmpresaAsync(empresaId, cancellationToken);
        return evento is null ? null : MapEvento(evento);
    }

    public async Task<RachaoPublicoResponse?> GetPublicoAsync(string token, CancellationToken cancellationToken)
    {
        var evento = await _repo.GetByTokenAsync(token, cancellationToken);
        return evento is null ? null : await MapPublicoAsync(evento, cancellationToken);
    }

    public async Task<RachaoPublicoResponse?> ConfirmarAsync(string token, ConfirmarPresencaRequest request, CancellationToken cancellationToken)
    {
        var evento = await _repo.GetByTokenAsync(token, cancellationToken);
        if (evento is null) return null;
        if (evento.SorteioFeito)
            throw new BusinessException("As confirmacoes ja foram encerradas (times sorteados).");

        var nome = (request.Nome ?? string.Empty).Trim();
        var empresa = (request.Empresa ?? string.Empty).Trim();
        var cpf = CpfHelper.Normalize(request.Cpf);
        if (nome.Length < 2)
            throw new BusinessException("Informe seu nome para confirmar.");
        if (empresa.Length < 2)
            throw new BusinessException("Informe a empresa em que voce joga.");
        if (!CpfHelper.IsValid(cpf))
            throw new BusinessException("Informe um CPF valido.");

        var chaveUnica = CriarChaveUnica(nome, empresa);
        var jaConfirmado = evento.Confirmacoes.Any(c => CriarChaveUnica(c.Nome, c.Empresa ?? string.Empty) == chaveUnica);
        if (jaConfirmado)
            throw new BusinessException("Voce ja confirmou presenca nesse rachao.");

        var cpfJaUsado = evento.Confirmacoes.Any(c => !string.IsNullOrEmpty(c.Cpf) && c.Cpf == cpf);
        if (cpfJaUsado)
            throw new BusinessException("Esse CPF ja confirmou presenca nesse rachao.");

        var confirmacao = new RachaoConfirmacao
        {
            Id = Guid.NewGuid(),
            RachaoEventoId = evento.Id,
            Nome = nome,
            Empresa = empresa,
            Cpf = cpf,
            Goleiro = request.Goleiro,
            ChaveUnica = chaveUnica,
            DataCriacao = DateTime.UtcNow
        };
        evento.Confirmacoes.Add(confirmacao);
        await _repo.AddConfirmacaoAsync(confirmacao, cancellationToken);

        return await MapPublicoAsync(evento, cancellationToken);
    }

    public async Task<RachaoPublicoResponse?> DesistirAsync(string token, DesistirPresencaRequest request, CancellationToken cancellationToken)
    {
        var evento = await _repo.GetByTokenAsync(token, cancellationToken);
        if (evento is null) return null;
        if (evento.SorteioFeito)
            throw new BusinessException("Os times ja foram sorteados, nao e possivel desistir.");

        var chaveUnica = CriarChaveUnica((request.Nome ?? string.Empty).Trim(), (request.Empresa ?? string.Empty).Trim());
        var confirmacao = evento.Confirmacoes.FirstOrDefault(c => CriarChaveUnica(c.Nome, c.Empresa ?? string.Empty) == chaveUnica);
        if (confirmacao is null)
            throw new BusinessException("Voce nao esta confirmado nesse rachao.");

        evento.Confirmacoes.Remove(confirmacao);
        await _repo.RemoveConfirmacaoAsync(confirmacao, cancellationToken);

        return await MapPublicoAsync(evento, cancellationToken);
    }

    public async Task<int> SortearPendentesAsync(CancellationToken cancellationToken)
    {
        var pendentes = await _repo.GetPendentesSorteioAsync(DateTime.UtcNow, cancellationToken);
        var total = 0;

        foreach (var evento in pendentes)
        {
            var porTime = evento.JogadoresPorTime;
            var confirmados = evento.Confirmacoes.Where(c => !c.Goleiro).Select(NomeComEmpresa).ToList();
            var numTimes = confirmados.Count / porTime;
            var deFora = confirmados.Count - (numTimes * porTime);

            if (numTimes >= 1)
            {
                var capacidade = numTimes * porTime;

                var anteriores = await _amistosoRepo.GetTimesAsync(evento.EmpresaId, cancellationToken);
                var chaveAnterior = ChaveFormacao(anteriores.Select(t => t.Jogadores.Select(j => j.Nome)));

                var grupos = new List<List<string>>();
                for (var tentativa = 0; tentativa < 12; tentativa++)
                {
                    var embaralhado = confirmados.OrderBy(_ => Guid.NewGuid()).Take(capacidade).ToList();
                    grupos = Enumerable.Range(0, numTimes)
                        .Select(i => embaralhado.Skip(i * porTime).Take(porTime).ToList())
                        .ToList();
                    if (ChaveFormacao(grupos) != chaveAnterior) break;
                }

                var dataSorteio = DateTime.UtcNow;
                var times = new List<TimeAmistoso>();
                for (var i = 0; i < numTimes; i++)
                {
                    var time = new TimeAmistoso
                    {
                        Id = Guid.NewGuid(),
                        EmpresaId = evento.EmpresaId,
                        Nome = $"Time {i + 1}",
                        Ordem = i + 1,
                        DataSorteio = dataSorteio,
                        Jogadores = new List<TimeAmistosoJogador>()
                    };
                    foreach (var nome in grupos[i])
                    {
                        time.Jogadores.Add(new TimeAmistosoJogador
                        {
                            Id = Guid.NewGuid(),
                            TimeAmistosoId = time.Id,
                            Nome = nome
                        });
                    }
                    times.Add(time);
                }

                await _amistosoRepo.ReplaceTimesAsync(evento.EmpresaId, times, cancellationToken);
            }

            evento.SorteioFeito = true;
            await _repo.SaveAsync(cancellationToken);
            total++;

            var excedentes = deFora > 0 ? confirmados.Skip(numTimes * porTime).ToList() : new List<string>();
            var mensagem = numTimes >= 1
                ? $"{confirmados.Count} confirmados: {numTimes} time(s) de {porTime}"
                  + (deFora > 0 ? $" - {deFora} ficaram de fora pro proximo rachao." : ".")
                : $"So {confirmados.Count} confirmado(s) - faltou gente pra formar um time de {porTime}.";

            await _notifier.SendToEmpresaAsync(evento.EmpresaId, new AppNotification(
                "rachao",
                "Times sorteados!",
                mensagem,
                "/amistoso"), cancellationToken);
        }

        return total;
    }

    public Task<int> LimparAntigosAsync(int dias, CancellationToken cancellationToken)
        => _repo.RemoverAntigosAsync(DateTime.UtcNow.AddDays(-dias), cancellationToken);

    private static RachaoEventoResponse MapEvento(RachaoEvento e)
        => new(
            e.Id, e.Token, e.HorarioEvento, e.NumeroTimes, e.SorteioFeito,
            e.Confirmacoes.OrderBy(c => c.DataCriacao)
                .Select(c => new RachaoConfirmacaoResponse(c.Id, c.Nome, c.Empresa, c.Goleiro)).ToList());

    private async Task<RachaoPublicoResponse> MapPublicoAsync(RachaoEvento e, CancellationToken cancellationToken)
    {
        var times = new List<TimeSorteadoResponse>();
        var excedentes = new List<string>();

        if (e.SorteioFeito && e.Confirmacoes.Count(c => !c.Goleiro) >= e.JogadoresPorTime)
        {
            var sorteados = await _amistosoRepo.GetTimesAsync(e.EmpresaId, cancellationToken);
            times = sorteados
                .OrderBy(t => t.Ordem)
                .Select(t => new TimeSorteadoResponse(t.Nome, t.Jogadores.Select(j => j.Nome).OrderBy(n => n).ToList()))
                .ToList();

            // Excedentes = confirmados de linha que nao estao em nenhum time sorteado (goleiros nunca entram no sorteio)
            var nomesSorteados = sorteados.SelectMany(t => t.Jogadores).Select(j => j.Nome).ToHashSet();
            excedentes = e.Confirmacoes
                .Where(c => !c.Goleiro)
                .Select(NomeComEmpresa)
                .Where(n => !nomesSorteados.Contains(n))
                .OrderBy(n => n)
                .ToList();
        }

        return new RachaoPublicoResponse(
            e.Token,
            e.Empresa?.Nome ?? "Rachao",
            e.HorarioEvento,
            e.JogadoresPorTime,
            e.NumeroTimes,
            e.SorteioFeito,
            e.Confirmacoes.OrderBy(c => c.DataCriacao).Select(c => new RachaoConfirmacaoResponse(c.Id, c.Nome, c.Empresa, c.Goleiro)).ToList(),
            times,
            excedentes);
    }

    private static string ChaveFormacao(IEnumerable<IEnumerable<string>> times)
    {
        var normalizados = times
            .Select(t => string.Join("|", t.Select(n => n.Trim().ToLowerInvariant()).OrderBy(n => n)))
            .OrderBy(s => s);
        return string.Join("#", normalizados);
    }

    private static string NomeComEmpresa(RachaoConfirmacao confirmacao)
        => string.IsNullOrWhiteSpace(confirmacao.Empresa)
            ? confirmacao.Nome
            : $"{confirmacao.Nome} - {confirmacao.Empresa}";

    private static string CriarChaveUnica(string nome, string empresa)
        => $"{NormalizarParte(nome)}|{NormalizarParte(empresa)}";

    private static string NormalizarParte(string valor)
    {
        var semAcentos = new StringBuilder();
        foreach (var c in valor.Trim().Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                semAcentos.Append(c);
        }

        var partes = semAcentos.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', partes);
    }
}