using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs;

public class BridgeHub : Hub
{
    public async Task Costam()
    {
        await Clients.All.SendAsync("Handle");
    }
}
