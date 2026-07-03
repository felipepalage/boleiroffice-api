using Microsoft.AspNetCore.SignalR;

namespace Boleiroffice.Api.Hubs;

public sealed class NotificationHub : Hub
{
    public async Task JoinGroup(string empresaId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"empresa-{empresaId}");
    }

    public async Task LeaveGroup(string empresaId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"empresa-{empresaId}");
    }
}
