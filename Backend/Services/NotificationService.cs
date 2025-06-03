using Backend.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<BridgeHub> _hubContext;

    public NotificationService(IHubContext<BridgeHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendJoinTableUpdate(string userId)
    {
        await _hubContext
            .Clients
            .User(userId)
            .SendAsync("ReceiveJoinTableUpdate");
    }

    public async Task SendStartOfGameUpdate(long tableId)
    {
        await _hubContext
            .Clients
            .Group(tableId.ToString())
            .SendAsync("ReceiveStartOfGameUpdate");
    }

    public async Task SendBidUpdate(long tableId)
    {
        await _hubContext
            .Clients
            .Group(tableId.ToString())
            .SendAsync("ReceiveBidUpdate");
    }

    public async Task SendCardPlayUpdate(long tableId)
    {
        await _hubContext
            .Clients
            .Group(tableId.ToString())
            .SendAsync("ReceiveCardPlayUpdate");
    }

    public async Task SendInvitationUpdate(string userId)
    {
        await _hubContext
            .Clients
            .User(userId)
            .SendAsync("ReceiveInvitationUpdate");
    }

    public async Task SendLeaveTableUpdate(string userId)
    {
        await _hubContext
           .Clients
           .User(userId)
           .SendAsync("ReceiveLeaveTableUpdate");
    }

    public async Task SendTableUpdate(long tableId)
    {
        await _hubContext
            .Clients
            .Group(tableId.ToString())
            .SendAsync("ReceiveTableUpdate");
    }

    public async Task SendDeleteTableUpdate(long tableId)
    {
        await _hubContext
            .Clients
            .Group(tableId.ToString())
            .SendAsync("ReceiveDeleteTableUpdate");
    }

    public async Task SendEndOfBiddingUpdate(long tableId)
    {
        await _hubContext
            .Clients
            .Group(tableId.ToString())
            .SendAsync("ReceiveEndOfBiddingUpdate");
    }

    public async Task SendEndOfGameUpdate(long tableId)
    {
        await _hubContext
            .Clients
            .Group(tableId.ToString())
            .SendAsync("ReceiveEndOfGameUpdate");
    }
}
