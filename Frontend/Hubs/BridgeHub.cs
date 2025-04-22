using Microsoft.AspNetCore.SignalR;

namespace Frontend.Hubs;
public class BridgeHub : Hub
{
    public async Task SendBidUpdated()
    {
        await Clients.All.SendAsync("ReceiveBidUpdate");
    }
}
