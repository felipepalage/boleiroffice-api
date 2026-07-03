using Boleiroffice.Application.Common.Models;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface ITimeRepository
{
    Task AddAsync(Time time, CancellationToken cancellationToken);
    Task UpdateAsync(Time time, CancellationToken cancellationToken);
    Task<bool> ExistsByNameWithinCompanyAsync(Guid empresaId, string nome, CancellationToken cancellationToken);
    Task<Time?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Time?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<Time>> GetPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken);
    Task DeleteAsync(Time time, CancellationToken cancellationToken);
}