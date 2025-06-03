using Microsoft.AspNetCore.SignalR.Client;

namespace Frontend.Services.Interfaces;

public interface ISignalRService
{
    Task StartConnectionAsync(string hubUrlRelativePath);
    Task StopConnectionAsync();
    Task JoinTableAsync(long tableId);
    HubConnectionState GetConnectionState();
}
