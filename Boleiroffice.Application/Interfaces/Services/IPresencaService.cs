using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Presenca;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IPresencaService
{
    Task<PresencaResponse> ConfirmarAsync(Guid desafioId, ConfirmarPresencaRequest request, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<PresencaDesafioResponse> GetByDesafioAsync(Guid desafioId, CancellationToken cancellationToken);
}
