using Boleiroffice.Application.Common.Models;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IQuadraRepository
{
    Task AddAsync(Quadra quadra, CancellationToken cancellationToken);
    Task UpdateAsync(Quadra quadra, CancellationToken cancellationToken);
    Task<Quadra?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<Quadra>> GetPagedAsync(string? bairro, PaginationParameters pagination, CancellationToken cancellationToken);
    Task AddAvaliacaoAsync(AvaliacaoQuadra avaliacao, CancellationToken cancellationToken);
    Task<bool> JaAvaliouAsync(Guid quadraId, Guid empresaId, CancellationToken cancellationToken);
}
