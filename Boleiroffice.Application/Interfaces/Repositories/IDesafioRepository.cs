using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Desafios;
using Boleiroffice.Application.DTOs.Feed;
using Boleiroffice.Application.DTOs.Ranking;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IDesafioRepository
{
    Task AddAsync(Desafio desafio, CancellationToken cancellationToken);
    Task<PagedResult<FeedJogoResponse>> GetFeedAsync(PaginationParameters pagination, CancellationToken cancellationToken);
    Task<Desafio?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<Desafio>> GetByTimeIdAsync(Guid timeId, PaginationParameters pagination, CancellationToken cancellationToken);
    Task<PagedResult<Desafio>> GetOpenAsync(string? bairro, DateOnly? dataJogo, PaginationParameters pagination, CancellationToken cancellationToken);
    Task<PagedResult<RankingResponse>> GetRankingAsync(PaginationParameters pagination, CancellationToken cancellationToken);
    Task<PagedResult<ArtilheiroRankingResponse>> GetTopScorersAsync(PaginationParameters pagination, CancellationToken cancellationToken);
    Task<PagedResult<ReputacaoRankingResponse>> GetReputationAsync(PaginationParameters pagination, CancellationToken cancellationToken);
    Task<PagedResult<SuggestedChallengeResponse>> GetSuggestedAsync(Guid timeId, DateOnly dataJogo, PaginationParameters pagination, CancellationToken cancellationToken);
    Task ReplaceScorersAsync(Guid desafioId, IEnumerable<GolPartida> gols, CancellationToken cancellationToken);
    Task<bool> TimeHasConflictAsync(Guid timeId, DateOnly dataJogo, Guid? ignoreDesafioId, CancellationToken cancellationToken);
    Task UpdateAsync(Desafio desafio, CancellationToken cancellationToken);
    Task<IReadOnlyList<Desafio>> GetExpiredPendingResultsAsync(DateTime expiredBefore, CancellationToken cancellationToken);
}
