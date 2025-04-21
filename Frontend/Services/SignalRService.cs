using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Frontend.Services;

public class SignalRService
{
    public HubConnection? HubConnection { get; private set;  }
     
    public async Task EnsureConnected(NavigationManager navigationManager)
    {
        if (HubConnection != null) return;

        HubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/gameHub"))
            .WithAutomaticReconnect()
            .Build();

        await HubConnection.StartAsync();
    }
}
