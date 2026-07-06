using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Amistoso;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class AmistosoService : IAmistosoService
{
    private readonly IAmistosoRepository _repository;
    private readonly INotificationService _notifier;

    public AmistosoService(IAmistosoRepository repository, INotificationService notifier)
    {
        _repository = repository;
        _notifier = notifier;
    }

    private Task NotificarPartidaAsync(Guid empresaId, PartidaAmistoso partida, CancellationToken cancellationToken)
        => _notifier.SendEventToEmpresaAsync(empresaId, "AmistosoAtualizado", new
        {
            partidaId = partida.Id,
            time1Gols = partida.Time1Gols,
            time2Gols = partida.Time2Gols,
            finalizada = partida.Finalizada
        }, cancellationToken);

    // ---------- Elenco ----------

    public async Task<IReadOnlyList<JogadorAmistosoResponse>> GetJogadoresAsync(Guid empresaId, CancellationToken cancellationToken)
    {
        var jogadores = await _repository.GetJogadoresAsync(empresaId, cancellationToken);
        return jogadores.Select(MapJogador).ToList();
    }

    public async Task<JogadorAmistosoResponse> AddJogadorAsync(Guid empresaId, JogadorAmistosoCreateRequest request, CancellationToken cancellationToken)
    {
        Guard.AgainstNullOrWhiteSpace(request.Nome, "Nome do jogador é obrigatório.");

        var jogador = new JogadorAmistoso
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Nome = request.Nome.Trim(),
            Ativo = true,
            PagouMensalidade = false,
            DataCriacao = DateTime.UtcNow
        };

        await _repository.AddJogadorAsync(jogador, cancellationToken);
        return MapJogador(jogador);
    }

    public async Task RemoverJogadorAsync(Guid empresaId, Guid jogadorId, CancellationToken cancellationToken)
    {
        var jogador = await _repository.GetJogadorByIdAsync(jogadorId, empresaId, cancellationToken)
            ?? throw new NotFoundException("Jogador não encontrado.");
        await _repository.RemoveJogadorAsync(jogador, cancellationToken);
    }

    public async Task<JogadorAmistosoResponse> AtualizarPagamentoAsync(Guid empresaId, Guid jogadorId, PagamentoUpdateRequest request, CancellationToken cancellationToken)
    {
        var jogador = await _repository.GetJogadorByIdAsync(jogadorId, empresaId, cancellationToken)
            ?? throw new NotFoundException("Jogador não encontrado.");

        jogador.PagouMensalidade = request.Pagou;
        await _repository.UpdateJogadorAsync(jogador, cancellationToken);
        return MapJogador(jogador);
    }

    public Task ZerarPagamentosAsync(Guid empresaId, CancellationToken cancellationToken)
        => _repository.ZerarPagamentosAsync(empresaId, cancellationToken);

    // ---------- Sorteio / Times ----------

    public async Task<IReadOnlyList<TimeAmistosoResponse>> SortearAsync(Guid empresaId, SortearRequest request, CancellationToken cancellationToken)
    {
        if (request.NumeroTimes < 2)
            throw new BusinessException("Escolha pelo menos 2 times para o sorteio.");

        var idsSelecionados = request.JogadorIds?.Distinct().ToList() ?? new List<Guid>();
        if (idsSelecionados.Count < request.NumeroTimes)
            throw new BusinessException("Selecione ao menos um jogador para cada time.");

        var elenco = await _repository.GetJogadoresAsync(empresaId, cancellationToken);
        var presentes = elenco.Where(j => idsSelecionados.Contains(j.Id)).ToList();
        if (presentes.Count < request.NumeroTimes)
            throw new BusinessException("Jogadores selecionados insuficientes para o sorteio.");

        // Embaralha (Fisher-Yates) e distribui em rodízio entre os times.
        var embaralhados = presentes.OrderBy(_ => Guid.NewGuid()).ToList();
        var dataSorteio = DateTime.UtcNow;
        var times = new List<TimeAmistoso>();
        for (var i = 0; i < request.NumeroTimes; i++)
        {
            times.Add(new TimeAmistoso
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                Nome = $"Time {i + 1}",
                Ordem = i + 1,
                DataSorteio = dataSorteio,
                Jogadores = new List<TimeAmistosoJogador>()
            });
        }

        for (var i = 0; i < embaralhados.Count; i++)
        {
            var time = times[i % request.NumeroTimes];
            time.Jogadores.Add(new TimeAmistosoJogador
            {
                Id = Guid.NewGuid(),
                TimeAmistosoId = time.Id,
                JogadorAmistosoId = embaralhados[i].Id,
                Nome = embaralhados[i].Nome
            });
        }

        await _repository.ReplaceTimesAsync(empresaId, times, cancellationToken);
        return times.Select(MapTime).ToList();
    }

    public async Task<IReadOnlyList<TimeAmistosoResponse>> GetTimesAsync(Guid empresaId, CancellationToken cancellationToken)
    {
        var times = await _repository.GetTimesAsync(empresaId, cancellationToken);
        return times.Select(MapTime).ToList();
    }

    // ---------- Partidas ----------

    public async Task<PartidaAmistosoResponse> IniciarPartidaAsync(Guid empresaId, IniciarPartidaRequest request, CancellationToken cancellationToken)
    {
        var partida = new PartidaAmistoso
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Time1Nome = string.IsNullOrWhiteSpace(request.Time1Nome) ? "Time 1" : request.Time1Nome.Trim(),
            Time2Nome = string.IsNullOrWhiteSpace(request.Time2Nome) ? "Time 2" : request.Time2Nome.Trim(),
            DataInicio = DateTime.UtcNow,
            Finalizada = false
        };

        await _repository.AddPartidaAsync(partida, cancellationToken);
        return MapPartida(partida);
    }

    public async Task<PartidaAmistosoResponse> RegistrarGolAsync(Guid empresaId, Guid partidaId, RegistrarGolRequest request, CancellationToken cancellationToken)
    {
        if (request.TimeNumero is not (1 or 2))
            throw new BusinessException("Time do gol deve ser 1 ou 2.");
        Guard.AgainstNullOrWhiteSpace(request.AutorNome, "Informe quem fez o gol.");

        var partida = await _repository.GetPartidaByIdAsync(partidaId, empresaId, cancellationToken)
            ?? throw new NotFoundException("Partida não encontrada.");
        if (partida.Finalizada)
            throw new BusinessException("Partida já finalizada.");

        var gol = new GolAmistoso
        {
            Id = Guid.NewGuid(),
            PartidaAmistosoId = partida.Id,
            EmpresaId = empresaId,
            TimeNumero = request.TimeNumero,
            AutorNome = request.AutorNome.Trim(),
            AssistenteNome = string.IsNullOrWhiteSpace(request.AssistenteNome) ? null : request.AssistenteNome.Trim(),
            DataCriacao = DateTime.UtcNow
        };
        partida.Gols.Add(gol);

        if (request.TimeNumero == 1) partida.Time1Gols++;
        else partida.Time2Gols++;

        // INSERT explícito do gol (Added). O incremento do placar na partida rastreada
        // é salvo no mesmo SaveChanges.
        await _repository.AddGolAsync(gol, cancellationToken);
        await NotificarPartidaAsync(empresaId, partida, cancellationToken);
        return MapPartida(partida);
    }

    public async Task<PartidaAmistosoResponse> AnularGolAsync(Guid empresaId, Guid partidaId, Guid golId, CancellationToken cancellationToken)
    {
        var partida = await _repository.GetPartidaByIdAsync(partidaId, empresaId, cancellationToken)
            ?? throw new NotFoundException("Partida não encontrada.");
        if (partida.Finalizada)
            throw new BusinessException("Partida já finalizada.");

        var gol = partida.Gols.FirstOrDefault(g => g.Id == golId)
            ?? throw new NotFoundException("Gol não encontrado.");

        if (gol.TimeNumero == 1) partida.Time1Gols = Math.Max(0, partida.Time1Gols - 1);
        else partida.Time2Gols = Math.Max(0, partida.Time2Gols - 1);
        partida.Gols.Remove(gol);

        await _repository.RemoveGolAsync(gol, cancellationToken);
        await NotificarPartidaAsync(empresaId, partida, cancellationToken);
        return MapPartida(partida);
    }

    public async Task<PartidaAmistosoResponse> FinalizarPartidaAsync(Guid empresaId, Guid partidaId, FinalizarPartidaRequest request, CancellationToken cancellationToken)
    {
        var partida = await _repository.GetPartidaByIdAsync(partidaId, empresaId, cancellationToken)
            ?? throw new NotFoundException("Partida não encontrada.");
        if (partida.Finalizada)
            throw new BusinessException("Partida já finalizada.");

        partida.Finalizada = true;
        partida.DataFim = DateTime.UtcNow;
        partida.DuracaoSegundos = request.DuracaoSegundos < 0 ? 0 : request.DuracaoSegundos;

        await _repository.UpdatePartidaAsync(partida, cancellationToken);
        await NotificarPartidaAsync(empresaId, partida, cancellationToken);
        return MapPartida(partida);
    }

    public async Task<PartidaAmistosoResponse> GetPartidaAsync(Guid empresaId, Guid partidaId, CancellationToken cancellationToken)
    {
        var partida = await _repository.GetPartidaByIdAsync(partidaId, empresaId, cancellationToken)
            ?? throw new NotFoundException("Partida não encontrada.");
        return MapPartida(partida);
    }

    public async Task<PagedResult<PartidaAmistosoResponse>> GetPartidasAsync(Guid empresaId, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var result = await _repository.GetPartidasPagedAsync(empresaId, pagination.Normalize(), cancellationToken);
        return result.Map(MapPartida);
    }

    // ---------- Ranking / resumo ----------

    public async Task<IReadOnlyList<ArtilheiroAmistosoResponse>> GetArtilheirosAsync(Guid empresaId, string periodo, CancellationToken cancellationToken)
    {
        var gols = await _repository.GetGolsByEmpresaAsync(empresaId, cancellationToken);
        return RankearArtilheiros(FiltrarPorPeriodo(gols, periodo));
    }

    public async Task<IReadOnlyList<ArtilheiroAmistosoResponse>> GetGarconsAsync(Guid empresaId, string periodo, CancellationToken cancellationToken)
    {
        var gols = await _repository.GetGolsByEmpresaAsync(empresaId, cancellationToken);
        return RankearGarcons(FiltrarPorPeriodo(gols, periodo));
    }

    public async Task<ResumoDiaResponse> GetResumoDiaAsync(Guid empresaId, DateOnly? data, CancellationToken cancellationToken)
    {
        var dia = data ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var partidas = await _repository.GetPartidasFinalizadasNoDiaAsync(empresaId, dia, cancellationToken);
        var gols = partidas.SelectMany(p => p.Gols).ToList();

        var artilheiro = RankearArtilheiros(gols).FirstOrDefault();
        var garcom = RankearGarcons(gols).FirstOrDefault();

        return new ResumoDiaResponse(
            dia.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            partidas.Count,
            gols.Count,
            artilheiro,
            garcom);
    }

    // ---------- Helpers ----------

    private static IEnumerable<GolAmistoso> FiltrarPorPeriodo(IEnumerable<GolAmistoso> gols, string? periodo)
    {
        var agora = DateTime.UtcNow;
        return periodo switch
        {
            "dia" => gols.Where(g => g.DataCriacao.Date == agora.Date),
            "mes" => gols.Where(g => g.DataCriacao.Year == agora.Year && g.DataCriacao.Month == agora.Month),
            _ => gols,
        };
    }

    private static IReadOnlyList<ArtilheiroAmistosoResponse> RankearArtilheiros(IEnumerable<GolAmistoso> gols)
        => gols
            .GroupBy(g => g.AutorNome)
            .Select(g => new ArtilheiroAmistosoResponse(g.Key, g.Count()))
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.Nome)
            .ToList();

    private static IReadOnlyList<ArtilheiroAmistosoResponse> RankearGarcons(IEnumerable<GolAmistoso> gols)
        => gols
            .Where(g => !string.IsNullOrWhiteSpace(g.AssistenteNome))
            .GroupBy(g => g.AssistenteNome!)
            .Select(g => new ArtilheiroAmistosoResponse(g.Key, g.Count()))
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.Nome)
            .ToList();

    private static JogadorAmistosoResponse MapJogador(JogadorAmistoso j)
        => new(j.Id, j.Nome, j.Ativo, j.PagouMensalidade, j.DataCriacao);

    private static TimeAmistosoResponse MapTime(TimeAmistoso t)
        => new(t.Id, t.Nome, t.Ordem, t.DataSorteio, t.Jogadores.Select(j => j.Nome).OrderBy(n => n).ToList());

    private static PartidaAmistosoResponse MapPartida(PartidaAmistoso p)
        => new(
            p.Id, p.Time1Nome, p.Time2Nome, p.Time1Gols, p.Time2Gols,
            p.DataInicio, p.DataFim, p.DuracaoSegundos, p.Finalizada,
            p.Gols.OrderBy(g => g.DataCriacao)
                .Select(g => new GolAmistosoResponse(g.Id, g.TimeNumero, g.AutorNome, g.AssistenteNome, g.DataCriacao))
                .ToList());
}
