using Boleiroffice.Api.Hubs;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Boleiroffice.Api.Services;

public sealed class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;
    private readonly IServiceProvider _services;

    public SignalRNotificationService(IHubContext<NotificationHub> hub, IServiceProvider services)
    {
        _hub = hub;
        _services = services;
    }

    public async Task SendToEmpresaAsync(Guid empresaId, AppNotification notification, CancellationToken cancellationToken = default)
    {
        await _hub.Clients
            .Group($"empresa-{empresaId}")
            .SendAsync("ReceiveNotification", notification, cancellationToken);

        await PersistAsync(empresaId, notification, cancellationToken);
    }

    public Task BroadcastAsync(AppNotification notification, CancellationToken cancellationToken = default)
        => _hub.Clients.All.SendAsync("ReceiveNotification", notification, cancellationToken);

    public Task SendEventToEmpresaAsync(Guid empresaId, string evento, object payload, CancellationToken cancellationToken = default)
        => _hub.Clients.Group($"empresa-{empresaId}").SendAsync(evento, payload, cancellationToken);

    private async Task PersistAsync(Guid empresaId, AppNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<INotificacaoRepository>();
            await repo.AddAsync(new Notificacao
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                Tipo = notification.Tipo,
                Titulo = notification.Titulo,
                Mensagem = notification.Mensagem,
                Url = notification.Url,
                Lida = false,
                DataCriacao = DateTime.UtcNow
            }, cancellationToken);
        }
        catch
        {
            // Don't let persistence failures break the real-time flow
        }
    }
}
