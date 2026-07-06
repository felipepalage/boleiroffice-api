using System.Globalization;
using System.Text;
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
        if (request.NumeroTimes < 2)
            throw new BusinessException("Escolha pelo menos 2 times.");

        var evento = new RachaoEvento
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Token = Guid.NewGuid().ToString("N")[..10],
            HorarioEvento = DateTime.SpecifyKind(request.HorarioEvento, DateTimeKind.Utc),
            NumeroTimes = request.NumeroTimes,
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
        if (nome.Length < 2)
            throw new BusinessException("Informe seu nome para confirmar.");
        if (empresa.Length < 2)
            throw new BusinessException("Informe a empresa em que voce joga.");

        var chaveUnica = CriarChaveUnica(nome, empresa);
        var jaConfirmado = evento.Confirmacoes.Any(c => CriarChaveUnica(c.Nome, c.Empresa ?? string.Empty) == chaveUnica);
        if (jaConfirmado)
            throw new BusinessException("Voce ja confirmou presenca nesse rachao.");

        var confirmacao = new RachaoConfirmacao
        {
            Id = Guid.NewGuid(),
            RachaoEventoId = evento.Id,
            Nome = nome,
            Empresa = empresa,
            ChaveUnica = chaveUnica,
            DataCriacao = DateTime.UtcNow
        };
        evento.Confirmacoes.Add(confirmacao);
        await _repo.AddConfirmacaoAsync(confirmacao, cancellationToken);

        return await MapPublicoAsync(evento, cancellationToken);
    }

    public async Task<int> SortearPendentesAsync(CancellationToken cancellationToken)
    {
        var pendentes = await _repo.GetPendentesSorteioAsync(DateTime.UtcNow, cancellationToken);
        var total = 0;

        foreach (var evento in pendentes)
        {
            var confirmados = evento.Confirmacoes
                .Select(NomeComEmpresa)
                .OrderBy(_ => Guid.NewGuid())
                .ToList();
            var numeroTimes = Math.Max(2, evento.NumeroTimes);

            if (confirmados.Count >= 1)
            {
                var dataSorteio = DateTime.UtcNow;
                var times = new List<TimeAmistoso>();
                for (var i = 0; i < numeroTimes; i++)
                {
                    times.Add(new TimeAmistoso
                    {
                        Id = Guid.NewGuid(),
                        EmpresaId = evento.EmpresaId,
                        Nome = $"Time {i + 1}",
                        Ordem = i + 1,
                        DataSorteio = dataSorteio,
                        Jogadores = new List<TimeAmistosoJogador>()
                    });
                }

                for (var i = 0; i < confirmados.Count; i++)
                {
                    var time = times[i % numeroTimes];
                    time.Jogadores.Add(new TimeAmistosoJogador
                    {
                        Id = Guid.NewGuid(),
                        TimeAmistosoId = time.Id,
                        Nome = confirmados[i]
                    });
                }

                await _amistosoRepo.ReplaceTimesAsync(evento.EmpresaId, times, cancellationToken);
            }

            evento.SorteioFeito = true;
            await _repo.SaveAsync(cancellationToken);
            total++;

            await _notifier.SendToEmpresaAsync(evento.EmpresaId, new AppNotification(
                "rachao",
                "Times sorteados!",
                $"{confirmados.Count} confirmado(s) - os times do rachao foram sorteados. Veja na aba Times.",
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
                .Select(c => new RachaoConfirmacaoResponse(c.Id, c.Nome, c.Empresa)).ToList());

    private async Task<RachaoPublicoResponse> MapPublicoAsync(RachaoEvento e, CancellationToken cancellationToken)
    {
        var times = new List<TimeSorteadoResponse>();
        if (e.SorteioFeito && e.Confirmacoes.Count > 0)
        {
            var sorteados = await _amistosoRepo.GetTimesAsync(e.EmpresaId, cancellationToken);
            times = sorteados
                .OrderBy(t => t.Ordem)
                .Select(t => new TimeSorteadoResponse(t.Nome, t.Jogadores.Select(j => j.Nome).OrderBy(n => n).ToList()))
                .ToList();
        }

        return new RachaoPublicoResponse(
            e.Token,
            e.Empresa?.Nome ?? "Rachao",
            e.HorarioEvento,
            e.NumeroTimes,
            e.SorteioFeito,
            e.Confirmacoes.OrderBy(c => c.DataCriacao).Select(c => new RachaoConfirmacaoResponse(c.Id, c.Nome, c.Empresa)).ToList(),
            times);
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