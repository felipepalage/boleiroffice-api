using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Chat;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IChatService
{
    Task<IReadOnlyList<MensagemResponse>> GetByDesafioAsync(Guid desafioId, CurrentUser currentUser, CancellationToken cancellationToken = default);
    Task<MensagemResponse> SendAsync(Guid desafioId, EnviarMensagemRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default);
}
