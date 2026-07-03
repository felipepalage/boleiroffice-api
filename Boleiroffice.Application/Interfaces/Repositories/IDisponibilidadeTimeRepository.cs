using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IDisponibilidadeTimeRepository
{
    Task<IReadOnlyList<DisponibilidadeTime>> GetByTimeAsync(Guid timeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DisponibilidadeTime>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<DisponibilidadeTime?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(DisponibilidadeTime item, CancellationToken cancellationToken = default);
    Task UpdateAsync(DisponibilidadeTime item, CancellationToken cancellationToken = default);
    Task DeleteAsync(DisponibilidadeTime item, CancellationToken cancellationToken = default);
    Task<bool> ExistsConflictAsync(Guid timeId, int diaSemana, TimeOnly horario, Guid? excludeId, CancellationToken cancellationToken = default);
}
