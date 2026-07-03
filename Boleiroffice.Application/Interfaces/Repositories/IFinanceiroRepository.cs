using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IFinanceiroRepository
{
    Task<IReadOnlyList<FinanceiroItem>> GetByTimeAsync(Guid timeId, CancellationToken cancellationToken = default);
    Task<FinanceiroItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(FinanceiroItem item, CancellationToken cancellationToken = default);
    Task UpdateAsync(FinanceiroItem item, CancellationToken cancellationToken = default);
    Task DeleteAsync(FinanceiroItem item, CancellationToken cancellationToken = default);
}
