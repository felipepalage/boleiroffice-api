using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IPostMuralRepository
{
    Task<IReadOnlyList<PostMural>> GetByEmpresaAsync(Guid empresaId, CancellationToken cancellationToken = default);
    Task<PostMural?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PostMural post, CancellationToken cancellationToken = default);
    Task DeleteAsync(PostMural post, CancellationToken cancellationToken = default);
}
