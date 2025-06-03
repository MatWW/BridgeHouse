using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Enums;
using Shared.Models;

namespace Frontend.ViewModels;

public class BridgeTableViewModel
{
    private readonly IApiService _apiService;
    private readonly ISignalRService _signalRService;

    public BridgeTableViewModel(IApiService apiService, ISignalRService signalRService)
    {
        _apiService = apiService;
        _signalRService = signalRService;
    }

    public BridgeTable? BridgeTable { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string NicknameInput { get; set; } = string.Empty;
    public Position SelectedPosition { get; set; } = Position.W;

    public event Func<Task>? OnStateChanged;

    public async Task InitializeAsync(long tableId)
    {
        await LoadBridgeTableAsync(tableId);
        UserId = await _apiService.GetSignedInUserIdAsync();

        if (_signalRService.GetConnectionState() != HubConnectionState.Connected)
            await _signalRService.StartConnectionAsync();

        await _signalRService.JoinTableAsync(tableId);
        _signalRService.OnReceiveTableUpdate += async () =>
        {
            await LoadBridgeTableAsync(tableId);
            if (OnStateChanged is not null)
                await OnStateChanged.Invoke();
        };
    }

    public async Task LoadBridgeTableAsync(long tableId) =>
        BridgeTable = await _apiService.GetBridgeTableAsync(tableId);
    
    public async Task SendInviteAsync()
    {
        var invitedUserId = await _apiService.GetIdByNickname(NicknameInput);
        await _apiService.SendInviteAsync(BridgeTable!.Id!.Value, invitedUserId, SelectedPosition);
    }

    public Task StartGameAsync() =>
        _apiService.StartGameAsync(BridgeTable!.Id!.Value, BridgeTable.Players);

    public Task DeleteTableAsync() =>
        _apiService.DeleteTableAsync(BridgeTable!.Id!.Value);

    public Task LeaveTableAsync() =>
        _apiService.LeaveTableAsync(BridgeTable!.Id!.Value);

    public Player? GetPlayerAtPosition(Position pos)
    {
        return BridgeTable?.Players.FirstOrDefault(p => p.Position == pos);
    }
}
