using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Ranking;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;

namespace Boleiroffice.Application.Services;

public sealed class RankingService : IRankingService
{
    private readonly ICacheService _cacheService;
    private readonly IDesafioRepository _desafioRepository;

    public RankingService(IDesafioRepository desafioRepository, ICacheService cacheService)
    {
        _desafioRepository = desafioRepository;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<RankingResponse>> GetAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var normalized = pagination.Normalize();
        var cacheKey = $"ranking:{normalized.Page}:{normalized.PageSize}:{normalized.DataInicio}";
        var cached = await _cacheService.GetAsync<PagedResult<RankingResponse>>(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        var ranking = await _desafioRepository.GetRankingAsync(normalized, cancellationToken);
        var response = Position(ranking);
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);

        return response;
    }

    public async Task<PagedResult<ArtilheiroRankingResponse>> GetScorersAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var normalized = pagination.Normalize();
        var cacheKey = $"ranking:artilheiros:{normalized.Page}:{normalized.PageSize}:{normalized.DataInicio}";
        var cached = await _cacheService.GetAsync<PagedResult<ArtilheiroRankingResponse>>(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        var ranking = await _desafioRepository.GetTopScorersAsync(normalized, cancellationToken);
        var response = Position(ranking);
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);

        return response;
    }

    public async Task<PagedResult<ReputacaoRankingResponse>> GetReputationAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var normalized = pagination.Normalize();
        var cacheKey = $"ranking:reputacao:{normalized.Page}:{normalized.PageSize}:{normalized.DataInicio}";
        var cached = await _cacheService.GetAsync<PagedResult<ReputacaoRankingResponse>>(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        var ranking = await _desafioRepository.GetReputationAsync(normalized, cancellationToken);
        var response = Position(ranking);
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);

        return response;
    }

    private static PagedResult<RankingResponse> Position(PagedResult<RankingResponse> ranking)
    {
        var positioned = ranking.Items.Select((item, index) =>
        {
            item.Posicao = ((ranking.Page - 1) * ranking.PageSize) + index + 1;
            return item;
        }).ToArray();

        return PagedResult<RankingResponse>.Create(positioned, ranking.Page, ranking.PageSize, ranking.TotalCount);
    }

    private static PagedResult<ArtilheiroRankingResponse> Position(PagedResult<ArtilheiroRankingResponse> ranking)
    {
        var positioned = ranking.Items.Select((item, index) =>
        {
            item.Posicao = ((ranking.Page - 1) * ranking.PageSize) + index + 1;
            return item;
        }).ToArray();

        return PagedResult<ArtilheiroRankingResponse>.Create(positioned, ranking.Page, ranking.PageSize, ranking.TotalCount);
    }

    private static PagedResult<ReputacaoRankingResponse> Position(PagedResult<ReputacaoRankingResponse> ranking)
    {
        var positioned = ranking.Items.Select((item, index) =>
        {
            item.Posicao = ((ranking.Page - 1) * ranking.PageSize) + index + 1;
            return item;
        }).ToArray();

        return PagedResult<ReputacaoRankingResponse>.Create(positioned, ranking.Page, ranking.PageSize, ranking.TotalCount);
    }
}
