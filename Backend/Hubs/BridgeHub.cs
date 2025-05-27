using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs;
public class BridgeHub : Hub
{
    public async Task SendBidUpdate()
    {
        await Clients.All.SendAsync("ReceiveBidUpdate");
    }

    public async Task SendCardPlayUpdate()
    {
        await Clients.All.SendAsync("ReceiveCardPlayUpdate");
    }

    public async Task SendGamePhaseUpdate()
    {
        await Clients.All.SendAsync("ReceiveGamePhaseUpdate");
    }
}
