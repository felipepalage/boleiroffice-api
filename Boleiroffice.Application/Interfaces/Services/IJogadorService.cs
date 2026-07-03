using Boleiroffice.Application.DTOs.Jogadores;

namespace Boleiroffice.Application.Interfaces.Services;

using Boleiroffice.Application.Common.Models;

public interface IJogadorService
{
    Task<JogadorResponse> CreateAsync(JogadorCreateRequest request, CurrentUser currentUser, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<JogadorResponse>> GetByTimeIdAsync(Guid timeId, CancellationToken cancellationToken);
    Task<JogadorPerfilResponse> GetPerfilAsync(Guid id, CancellationToken cancellationToken);
}
