using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Boleiroffice.Api.Hubs;

[Authorize]
public sealed class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var empresaId = Context.User?.FindFirst("empresa_id")?.Value;
        if (!string.IsNullOrEmpty(empresaId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"empresa-{empresaId}");
        }

        await base.OnConnectedAsync();
    }
}
