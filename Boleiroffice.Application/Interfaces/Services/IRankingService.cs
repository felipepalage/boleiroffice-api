using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Ranking;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IRankingService
{
    Task<PagedResult<RankingResponse>> GetAsync(PaginationParameters pagination, CancellationToken cancellationToken);
    Task<PagedResult<ArtilheiroRankingResponse>> GetScorersAsync(PaginationParameters pagination, CancellationToken cancellationToken);
    Task<PagedResult<ReputacaoRankingResponse>> GetReputationAsync(PaginationParameters pagination, CancellationToken cancellationToken);
}
