using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface ITemporadaRepository
{
    Task<IReadOnlyList<Temporada>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Temporada?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Temporada?> GetAtivaAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Temporada temporada, CancellationToken cancellationToken = default);
    Task UpdateAsync(Temporada temporada, CancellationToken cancellationToken = default);
    Task DeleteAsync(Temporada temporada, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNomeAsync(string nome, CancellationToken cancellationToken = default);
}
