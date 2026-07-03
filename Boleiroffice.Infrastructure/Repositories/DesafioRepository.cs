using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Desafios;
using Boleiroffice.Application.DTOs.Feed;
using Boleiroffice.Application.DTOs.Ranking;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Domain.Enums;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class DesafioRepository : IDesafioRepository
{
    private readonly ApplicationDbContext _context;

    public DesafioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Desafio desafio, CancellationToken cancellationToken)
    {
        await _context.Desafios.AddAsync(desafio, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Desafio desafio, CancellationToken cancellationToken)
    {
        _context.Desafios.Update(desafio);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceScorersAsync(Guid desafioId, IEnumerable<GolPartida> gols, CancellationToken cancellationToken)
    {
        var atuais = await _context.GolsPartida.Where(x => x.DesafioId == desafioId).ToArrayAsync(cancellationToken);
        _context.GolsPartida.RemoveRange(atuais);
        await _context.GolsPartida.AddRangeAsync(gols, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<Desafio?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Desafios
            .Include(x => x.TimeCriador)!
                .ThenInclude(x => x!.Empresa)
            .Include(x => x.TimeDesafiante)!
                .ThenInclude(x => x!.Empresa)
            .Include(x => x.Gols)!
                .ThenInclude(x => x.Time)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Desafio>> GetOpenAsync(string? bairro, DateOnly? dataJogo, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = BaseDesafioQuery()
            .Where(x => x.Status == DesafioStatus.Aberto);

        if (!string.IsNullOrWhiteSpace(bairro))
        {
            var normalizedBairro = bairro.Trim().ToLower();
            query = query.Where(x => x.Bairro.ToLower().Contains(normalizedBairro));
        }

        if (dataJogo.HasValue)
        {
            query = query.Where(x => x.DataJogo == dataJogo.Value);
        }

        query = query.OrderBy(x => x.DataJogo).ThenBy(x => x.HoraJogo);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);

        return PagedResult<Desafio>.Create(items, pagination.Page, pagination.PageSize, totalCount);
    }

    public async Task<PagedResult<Desafio>> GetByTimeIdAsync(Guid timeId, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = BaseDesafioQuery()
            .Where(x => x.TimeCriadorId == timeId || x.TimeDesafianteId == timeId)
            .OrderByDescending(x => x.DataJogo)
            .ThenByDescending(x => x.HoraJogo);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);

        return PagedResult<Desafio>.Create(items, pagination.Page, pagination.PageSize, totalCount);
    }

    public async Task<bool> TimeHasConflictAsync(Guid timeId, DateOnly dataJogo, Guid? ignoreDesafioId, CancellationToken cancellationToken)
    {
        return await _context.Desafios.AnyAsync(x =>
            (!ignoreDesafioId.HasValue || x.Id != ignoreDesafioId.Value) &&
            x.DataJogo == dataJogo &&
            (x.Status == DesafioStatus.Aberto || x.Status == DesafioStatus.Aceito || x.Status == DesafioStatus.ResultadoPendente) &&
            (x.TimeCriadorId == timeId || x.TimeDesafianteId == timeId), cancellationToken);
    }

    public async Task<PagedResult<FeedJogoResponse>> GetFeedAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = _context.Desafios
            .AsNoTracking()
            .Include(x => x.TimeCriador)!
                .ThenInclude(x => x!.Empresa)
            .Include(x => x.TimeDesafiante)!
                .ThenInclude(x => x!.Empresa)
            .Include(x => x.Gols)
                .ThenInclude(x => x.Time)
            .Where(x => x.Status == DesafioStatus.Finalizado && x.TimeDesafianteId.HasValue)
            .OrderByDescending(x => x.DataJogo)
            .ThenByDescending(x => x.HoraJogo);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);

        var response = items.Select(MapFeedItem).ToArray();

        // Auto-mark flags: most recent = DestaqueDoDia, highest-scoring = JogoDaRodada
        if (response.Length > 0)
        {
            response[0].DestaqueDoDia = true;
            var topScoring = response.OrderByDescending(r => (r.PlacarCriador ?? 0) + (r.PlacarDesafiante ?? 0)).First();
            topScoring.JogoDaRodada = true;
        }

        return PagedResult<FeedJogoResponse>.Create(response, pagination.Page, pagination.PageSize, totalCount);
    }

    public async Task<PagedResult<RankingResponse>> GetRankingAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        // Load every team regardless of match history
        var allTimes = await _context.Times
            .AsNoTracking()
            .Include(x => x.Empresa)
            .ToArrayAsync(cancellationToken);

        // Aggregate match stats only from finalized games
        var finalizedQuery = _context.Desafios
            .AsNoTracking()
            .Where(x => x.Status == DesafioStatus.Finalizado && x.TimeDesafianteId.HasValue
                && (pagination.DataInicio == null || x.DataJogo >= pagination.DataInicio));

        var criadoresStats = await finalizedQuery.Select(x => new
        {
            TimeId = x.TimeCriadorId,
            GolsPro = x.PlacarCriador ?? 0,
            GolsContra = x.PlacarDesafiante ?? 0,
            Vitoria = (x.PlacarCriador ?? 0) > (x.PlacarDesafiante ?? 0) ? 1 : 0,
            Empate = (x.PlacarCriador ?? 0) == (x.PlacarDesafiante ?? 0) ? 1 : 0,
            Derrota = (x.PlacarCriador ?? 0) < (x.PlacarDesafiante ?? 0) ? 1 : 0
        }).ToArrayAsync(cancellationToken);

        var desafiantesStats = await finalizedQuery.Select(x => new
        {
            TimeId = x.TimeDesafianteId!.Value,
            GolsPro = x.PlacarDesafiante ?? 0,
            GolsContra = x.PlacarCriador ?? 0,
            Vitoria = (x.PlacarDesafiante ?? 0) > (x.PlacarCriador ?? 0) ? 1 : 0,
            Empate = (x.PlacarDesafiante ?? 0) == (x.PlacarCriador ?? 0) ? 1 : 0,
            Derrota = (x.PlacarDesafiante ?? 0) < (x.PlacarCriador ?? 0) ? 1 : 0
        }).ToArrayAsync(cancellationToken);

        var statsDict = criadoresStats
            .Concat(desafiantesStats)
            .GroupBy(x => x.TimeId)
            .ToDictionary(
                g => g.Key,
                g => (
                    Jogos: g.Count(),
                    Vitorias: g.Sum(x => x.Vitoria),
                    Empates: g.Sum(x => x.Empate),
                    Derrotas: g.Sum(x => x.Derrota),
                    GolsPro: g.Sum(x => x.GolsPro),
                    GolsContra: g.Sum(x => x.GolsContra)
                )
            );

        var ranking = allTimes.Select(time =>
        {
            var s = statsDict.GetValueOrDefault(time.Id);
            return new RankingResponse
            {
                TimeId = time.Id,
                Time = time.Nome,
                Empresa = time.Empresa?.Nome ?? string.Empty,
                TimeFotoUrl = time.FotoUrl,
                EmpresaLogoUrl = time.Empresa?.LogoUrl,
                EscudoShape = time.EscudoShape,
                CorPrimaria = time.CorPrimaria,
                CorSecundaria = time.CorSecundaria,
                Jogos = s.Jogos,
                Vitorias = s.Vitorias,
                Empates = s.Empates,
                Derrotas = s.Derrotas,
                GolsPro = s.GolsPro,
                GolsContra = s.GolsContra,
                Saldo = s.GolsPro - s.GolsContra,
                Pontos = (s.Vitorias * 3) + s.Empates
            };
        })
        .OrderByDescending(x => x.Pontos)
        .ThenByDescending(x => x.Saldo)
        .ThenByDescending(x => x.GolsPro)
        .ThenBy(x => x.Time)
        .ToArray();

        var totalCount = ranking.Length;
        var pageItems = ranking
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArray();

        return PagedResult<RankingResponse>.Create(pageItems, pagination.Page, pagination.PageSize, totalCount);
    }

    public async Task<PagedResult<ArtilheiroRankingResponse>> GetTopScorersAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = _context.GolsPartida
            .AsNoTracking()
            .Where(x => pagination.DataInicio == null || x.Desafio!.DataJogo >= pagination.DataInicio)
            .GroupBy(x => new
            {
                x.TimeId,
                Time = x.Time!.Nome,
                Empresa = x.Time!.Empresa!.Nome,
                TimeFotoUrl = x.Time!.FotoUrl,
                EmpresaLogoUrl = x.Time!.Empresa!.LogoUrl,
                EscudoShape = x.Time!.EscudoShape,
                CorPrimaria = x.Time!.CorPrimaria,
                CorSecundaria = x.Time!.CorSecundaria,
                x.NomeAutor
            })
            .Select(g => new ArtilheiroRankingResponse
            {
                TimeId = g.Key.TimeId,
                Time = g.Key.Time,
                Empresa = g.Key.Empresa,
                TimeFotoUrl = g.Key.TimeFotoUrl,
                EmpresaLogoUrl = g.Key.EmpresaLogoUrl,
                EscudoShape = g.Key.EscudoShape,
                CorPrimaria = g.Key.CorPrimaria,
                CorSecundaria = g.Key.CorSecundaria,
                NomeAutor = g.Key.NomeAutor,
                Gols = g.Sum(x => x.QuantidadeGols),
                JogosComGol = g.Count()
            })
            .OrderByDescending(x => x.Gols)
            .ThenByDescending(x => x.JogosComGol)
            .ThenBy(x => x.NomeAutor);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);

        return PagedResult<ArtilheiroRankingResponse>.Create(items, pagination.Page, pagination.PageSize, totalCount);
    }

    public async Task<PagedResult<ReputacaoRankingResponse>> GetReputationAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var items = await _context.Times
            .AsNoTracking()
            .Include(x => x.Empresa)
            .Include(x => x.DesafiosCriados)
            .Include(x => x.DesafiosRecebidos)
            .ToArrayAsync(cancellationToken);

        var ranking = items.Select(time =>
        {
            var desafios = time.DesafiosCriados.Concat(time.DesafiosRecebidos)
                .Where(x => x.Status == DesafioStatus.Finalizado || x.Status == DesafioStatus.Cancelado || x.Status == DesafioStatus.Aceito || x.Status == DesafioStatus.ResultadoPendente)
                .ToArray();

            var jogosConfirmados = desafios.Count(x => x.DataAceite.HasValue || x.Status == DesafioStatus.Finalizado);
            var comparecimentos = desafios.Count(x => x.Status == DesafioStatus.Finalizado);
            var cancelamentosTardios = desafios.Count(x =>
                x.Status == DesafioStatus.Cancelado &&
                x.DataCancelamento.HasValue &&
                CombineDateTime(x.DataJogo, x.HoraJogo).Subtract(x.DataCancelamento.Value).TotalHours <= 12);
            var confirmacoesRapidas = desafios.Count(x =>
                x.Status == DesafioStatus.Finalizado &&
                x.DataResultadoConfirmadoEm.HasValue &&
                x.DataPropostaResultado.HasValue &&
                x.DataResultadoConfirmadoEm.Value.Subtract(x.DataPropostaResultado.Value).TotalHours <= 12);

            var indice = Math.Clamp(70 + (comparecimentos * 4) + (confirmacoesRapidas * 3) - (cancelamentosTardios * 15), 0, 100);

            return new ReputacaoRankingResponse
            {
                TimeId = time.Id,
                Time = time.Nome,
                Empresa = time.Empresa?.Nome ?? string.Empty,
                TimeFotoUrl = time.FotoUrl,
                EmpresaLogoUrl = time.Empresa?.LogoUrl,
                EscudoShape = time.EscudoShape,
                CorPrimaria = time.CorPrimaria,
                CorSecundaria = time.CorSecundaria,
                JogosConfirmados = jogosConfirmados,
                Comparecimentos = comparecimentos,
                CancelamentosTardios = cancelamentosTardios,
                ConfirmacoesRapidas = confirmacoesRapidas,
                IndiceConfiabilidade = indice
            };
        })
        .OrderByDescending(x => x.IndiceConfiabilidade)
        .ThenByDescending(x => x.Comparecimentos)
        .ThenBy(x => x.Time)
        .ToArray();

        var totalCount = ranking.Length;
        var pageItems = ranking
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArray();

        return PagedResult<ReputacaoRankingResponse>.Create(pageItems, pagination.Page, pagination.PageSize, totalCount);
    }

    public async Task<PagedResult<SuggestedChallengeResponse>> GetSuggestedAsync(Guid timeId, DateOnly dataJogo, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var timeReferencia = await _context.Times
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == timeId, cancellationToken);

        if (timeReferencia is null)
        {
            return PagedResult<SuggestedChallengeResponse>.Create(Array.Empty<SuggestedChallengeResponse>(), pagination.Page, pagination.PageSize, 0);
        }

        var candidatosQuery = _context.Times
            .AsNoTracking()
            .Where(x => x.Id != timeId && x.EmpresaId != timeReferencia.EmpresaId)
            .Select(x => new
            {
                x.Id,
                x.Nome,
                Empresa = x.Empresa!.Nome,
                x.BairroBase,
                x.Nivel,
                PossuiConflito = _context.Desafios.Any(d =>
                    d.DataJogo == dataJogo &&
                    (d.Status == DesafioStatus.Aberto || d.Status == DesafioStatus.Aceito || d.Status == DesafioStatus.ResultadoPendente) &&
                    (d.TimeCriadorId == x.Id || d.TimeDesafianteId == x.Id))
            })
            .Where(x => !x.PossuiConflito)
            .OrderByDescending(x => x.BairroBase == timeReferencia.BairroBase)
            .ThenBy(x => Math.Abs(x.Nivel - timeReferencia.Nivel))
            .ThenBy(x => x.Nome);

        var totalCount = await candidatosQuery.CountAsync(cancellationToken);
        var items = await candidatosQuery
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);

        var response = items.Select(x =>
        {
            var scoreBairro = string.Equals(x.BairroBase, timeReferencia.BairroBase, StringComparison.OrdinalIgnoreCase) ? 60 : 25;
            var scoreNivel = Math.Max(0, 40 - (Math.Abs(x.Nivel - timeReferencia.Nivel) * 10));
            var mesmoBairro = string.Equals(x.BairroBase, timeReferencia.BairroBase, StringComparison.OrdinalIgnoreCase);

            return new SuggestedChallengeResponse
            {
                TimeId = x.Id,
                NomeTime = x.Nome,
                Empresa = x.Empresa,
                BairroBase = x.BairroBase,
                Nivel = x.Nivel,
                DisponivelNaData = true,
                ScoreCompatibilidade = scoreBairro + scoreNivel,
                Motivo = mesmoBairro
                    ? "Mesmo bairro e nivel competitivo semelhante."
                    : "Boa proximidade de nivel e agenda disponivel."
            };
        }).ToArray();

        return PagedResult<SuggestedChallengeResponse>.Create(response, pagination.Page, pagination.PageSize, totalCount);
    }

    public Task<IReadOnlyList<Desafio>> GetExpiredPendingResultsAsync(DateTime expiredBefore, CancellationToken cancellationToken)
        => _context.Desafios
            .Where(d => d.Status == DesafioStatus.ResultadoPendente && d.DataPropostaResultado.HasValue && d.DataPropostaResultado.Value <= expiredBefore)
            .ToArrayAsync(cancellationToken)
            .ContinueWith<IReadOnlyList<Desafio>>(t => t.Result, cancellationToken);

    private IQueryable<Desafio> BaseDesafioQuery()
    {
        return _context.Desafios
            .AsNoTracking()
            .Include(x => x.TimeCriador)!
                .ThenInclude(x => x!.Empresa)
            .Include(x => x.TimeDesafiante)!
                .ThenInclude(x => x!.Empresa)
            .Include(x => x.Gols)!
                .ThenInclude(x => x!.Time);
    }

    private static FeedJogoResponse MapFeedItem(Desafio x)
    {
        var placarCriador = x.PlacarCriador ?? 0;
        var placarDesafiante = x.PlacarDesafiante ?? 0;
        var artilheiros = x.Gols
            .OrderByDescending(g => g.QuantidadeGols)
            .ThenBy(g => g.NomeAutor)
            .Select(g => g.QuantidadeGols > 1 ? g.NomeAutor + " (" + g.QuantidadeGols + ")" : g.NomeAutor)
            .ToArray();

        return new FeedJogoResponse
        {
            Id = x.Id,
            DataJogo = x.DataJogo,
            HoraJogo = x.HoraJogo,
            Local = x.Local,
            Bairro = x.Bairro,
            TimeCriador = x.TimeCriador!.Nome,
            EmpresaCriadora = x.TimeCriador!.Empresa!.Nome,
            TimeCriadorFotoUrl = x.TimeCriador!.FotoUrl,
            EmpresaCriadoraLogoUrl = x.TimeCriador!.Empresa!.LogoUrl,
            TimeCriadorEscudoShape = x.TimeCriador!.EscudoShape,
            TimeCriadorCorPrimaria = x.TimeCriador!.CorPrimaria,
            TimeCriadorCorSecundaria = x.TimeCriador!.CorSecundaria,
            TimeDesafiante = x.TimeDesafiante!.Nome,
            EmpresaDesafiante = x.TimeDesafiante!.Empresa!.Nome,
            TimeDesafianteFotoUrl = x.TimeDesafiante!.FotoUrl,
            EmpresaDesafianteLogoUrl = x.TimeDesafiante!.Empresa!.LogoUrl,
            TimeDesafianteEscudoShape = x.TimeDesafiante!.EscudoShape,
            TimeDesafianteCorPrimaria = x.TimeDesafiante!.CorPrimaria,
            TimeDesafianteCorSecundaria = x.TimeDesafiante!.CorSecundaria,
            Manchete = placarCriador > placarDesafiante
                ? x.TimeCriador!.Nome + " vence " + x.TimeDesafiante!.Nome
                : placarCriador < placarDesafiante
                    ? x.TimeDesafiante!.Nome + " bate " + x.TimeCriador!.Nome
                    : x.TimeCriador!.Nome + " e " + x.TimeDesafiante!.Nome + " empatam",
            Resumo = BuildResumo(x, placarCriador, placarDesafiante),
            ChamadaEditorial = BuildChamadaEditorial(x, placarCriador, placarDesafiante),
            ArtilheirosResumo = artilheiros.Length == 0 ? null : "Gols de " + string.Join(", ", artilheiros) + ".",
            PlacarCriador = x.PlacarCriador,
            PlacarDesafiante = x.PlacarDesafiante,
            Status = x.Status
        };
    }

    private static string BuildResumo(Desafio x, int placarCriador, int placarDesafiante)
    {
        if (placarCriador > placarDesafiante)
        {
            return x.TimeCriador!.Empresa!.Nome + " confirmou a vitoria por " + placarCriador + " a " + placarDesafiante + " em " + x.Local + ".";
        }

        if (placarCriador < placarDesafiante)
        {
            return x.TimeDesafiante!.Empresa!.Nome + " saiu com a vitoria por " + placarDesafiante + " a " + placarCriador + " em " + x.Local + ".";
        }

        return "As equipes dividiram os pontos apos empate por " + placarCriador + " a " + placarDesafiante + " em " + x.Local + ".";
    }

    private static string BuildChamadaEditorial(Desafio x, int placarCriador, int placarDesafiante)
    {
        var totalGols = placarCriador + placarDesafiante;
        var saldo = Math.Abs(placarCriador - placarDesafiante);

        if (saldo == 0 && totalGols >= 4)
        {
            return "Duelo equilibrado e cheio de gols movimenta o noticiario corporativo.";
        }

        if (saldo == 1)
        {
            return "Partida decidida no detalhe reforca o clima de rivalidade entre as empresas.";
        }

        if (totalGols >= 5)
        {
            return "Ataques inspirados transformam o amistoso em um placar de repercussao imediata.";
        }

        return "Resultado confirmado alimenta a corrida por prestigio no futebol corporativo.";
    }

    private static DateTime CombineDateTime(DateOnly date, TimeOnly time)
    {
        return date.ToDateTime(time, DateTimeKind.Utc);
    }
}
