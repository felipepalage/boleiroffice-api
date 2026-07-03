using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IComentarioFeedRepository
{
    Task<IReadOnlyList<ComentarioFeed>> GetByDesafioAsync(Guid desafioId, CancellationToken cancellationToken = default);
    Task<ComentarioFeed?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ComentarioFeed comentario, CancellationToken cancellationToken = default);
    Task DeleteAsync(ComentarioFeed comentario, CancellationToken cancellationToken = default);
}
