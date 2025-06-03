using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

public class SignalRService : IAsyncDisposable, ISignalRService
{
    private HubConnection? _hubConnection;

    public event Func<Task>? OnReceiveTableUpdate;
    public event Func<Task>? OnReceiveInvitationUpdate;
    public event Func<Task>? OnReceiveJoinTableUpdate;
    public event Func<Task>? OnReceiveLeaveTableUpdate;
    public event Func<Task>? OnReceiveDeleteTableUpdate;
    public event Func<Task>? OnReceiveStartOfGameUpdate;
    public event Func<Task>? OnReceiveBidUpdate;
    public event Func<Task>? OnReceiveEndOfBiddingUpdate;
    public event Func<Task>? OnReceiveCardPlayUpdate;
    public event Func<Task>? OnReceiveEndOfGameUpdate;

    public async Task StartConnectionAsync(string hubUrlRelativePath = "/gamehub")
    {
        if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
        {
            return;
        }

        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }

        var hubUrl = "https://localhost:7200" + hubUrlRelativePath;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On("ReceiveTableUpdate", async () =>
        {
            if (OnReceiveTableUpdate != null)
                await OnReceiveTableUpdate.Invoke();
        });
        _hubConnection.On("ReceiveInvitationUpdate", async () =>
        {
            if (OnReceiveInvitationUpdate != null)
                await OnReceiveInvitationUpdate.Invoke();
        });
        _hubConnection.On("ReceiveJoinTableUpdate", async () =>
        {
            if (OnReceiveJoinTableUpdate != null)
                await OnReceiveJoinTableUpdate.Invoke();
        });
        _hubConnection.On("ReceiveLeaveTableUpdate", async () =>
        {
            if (OnReceiveLeaveTableUpdate != null)
                await OnReceiveLeaveTableUpdate.Invoke();
        });
        _hubConnection.On("ReceiveDeleteTableUpdate", async () =>
        {
            if (OnReceiveDeleteTableUpdate != null)
                await OnReceiveDeleteTableUpdate.Invoke();
        });
        _hubConnection.On("ReceiveStartOfGameUpdate", async () =>
        {
            if (OnReceiveStartOfGameUpdate != null)
                await OnReceiveStartOfGameUpdate.Invoke();
        });
        _hubConnection.On("ReceiveBidUpdate", async () =>
        {
            if (OnReceiveBidUpdate != null)
                await OnReceiveBidUpdate.Invoke();
        });
        _hubConnection.On("ReceiveEndOfBiddingUpdate", async () =>
        {
            if (OnReceiveEndOfBiddingUpdate != null)
                await OnReceiveEndOfBiddingUpdate.Invoke();
        });
        _hubConnection.On("ReceiveCardPlayUpdate", async () =>
        {
            if (OnReceiveCardPlayUpdate != null)
                await OnReceiveCardPlayUpdate.Invoke();
        });
        _hubConnection.On("ReceiveEndOfGameUpdate", async () =>
        {
            if (OnReceiveEndOfGameUpdate != null)
                await OnReceiveEndOfGameUpdate.Invoke();
        });
        
        await _hubConnection.StartAsync();
    }

    public async Task StopConnectionAsync()
    {
        if (_hubConnection != null && _hubConnection.State != HubConnectionState.Disconnected)
        {
            await _hubConnection.StopAsync();
        }
    }

    public async Task JoinTableAsync(long tableId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("JoinTable", tableId.ToString());
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }

    public HubConnectionState GetConnectionState()
    {
        return _hubConnection?.State ?? HubConnectionState.Disconnected;
    }
}