using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Mural;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IPostMuralService
{
    Task<IReadOnlyList<PostMuralResponse>> GetByEmpresaAsync(Guid empresaId, CancellationToken cancellationToken = default);
    Task<PostMuralResponse> CreateAsync(Guid empresaId, PostMuralRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CurrentUser currentUser, CancellationToken cancellationToken = default);
}
