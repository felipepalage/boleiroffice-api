using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IVotoMvpRepository
{
    Task<VotoMvp?> GetByDesafioAndEmpresaAsync(Guid desafioId, Guid empresaId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VotoMvp>> GetByDesafioAsync(Guid desafioId, CancellationToken cancellationToken = default);
    Task UpsertAsync(VotoMvp voto, CancellationToken cancellationToken = default);
}
