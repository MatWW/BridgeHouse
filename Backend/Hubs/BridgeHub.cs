using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs;
public class BridgeHub : Hub
{
    public async Task JoinTable(string tableId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, tableId);
    }
}
