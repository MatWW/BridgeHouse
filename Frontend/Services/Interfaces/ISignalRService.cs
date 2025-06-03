using Microsoft.AspNetCore.SignalR.Client;

namespace Frontend.Services.Interfaces;

public interface ISignalRService
{
    event Func<Task>? OnReceiveTableUpdate;
    event Func<Task>? OnReceiveInvitationUpdate;
    event Func<Task>? OnReceiveJoinTableUpdate;
    event Func<Task>? OnReceiveLeaveTableUpdate;
    event Func<Task>? OnReceiveDeleteTableUpdate;
    event Func<Task>? OnReceiveStartOfGameUpdate;
    event Func<Task>? OnReceiveBidUpdate;
    event Func<Task>? OnReceiveEndOfBiddingUpdate;
    event Func<Task>? OnReceiveCardPlayUpdate;
    event Func<Task>? OnReceiveEndOfGameUpdate;

    Task StartConnectionAsync(string hubUrlRelativePath = "/gamehub");
    Task StopConnectionAsync();
    Task JoinTableAsync(long tableId);
    HubConnectionState GetConnectionState();
}
