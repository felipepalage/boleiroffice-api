using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Feed;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IComentarioFeedService
{
    Task<IReadOnlyList<ComentarioResponse>> GetByDesafioAsync(Guid desafioId, CancellationToken cancellationToken = default);
    Task<ComentarioResponse> AddAsync(Guid desafioId, ComentarRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid comentarioId, CurrentUser currentUser, CancellationToken cancellationToken = default);
}
