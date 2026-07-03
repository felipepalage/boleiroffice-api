namespace Boleiroffice.Application.Interfaces.Services;

public sealed record AppNotification(
    string Tipo,
    string Titulo,
    string Mensagem,
    string? Url = null,
    DateTime? DataHora = null);

public interface INotificationService
{
    Task SendToEmpresaAsync(Guid empresaId, AppNotification notification, CancellationToken cancellationToken = default);
    Task BroadcastAsync(AppNotification notification, CancellationToken cancellationToken = default);
}
