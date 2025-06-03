using Shared.Enums;
using Frontend.Services.Interfaces;

namespace Frontend.ViewModels;

public class MainPageViewModel
{
    private readonly IApiService _apiService;
    private readonly ISignalRService _signalRService;

    public PlayerState PlayerState { get; private set; } = PlayerState.NONE;
    public GamePhase? GamePhase { get; private set; }
    public long? TableId { get; private set; }
    public long? GameId { get; private set; }
    public long? InviteTableId { get; private set; }

    public event Func<Task>? OnStateChanged;

    public MainPageViewModel(IApiService apiService, ISignalRService signalRService)
    {
        _apiService = apiService;
        _signalRService = signalRService;
    }

    public async Task InitializeAsync()
    {
        _signalRService.OnReceiveLeaveTableUpdate += RefreshState;
        _signalRService.OnReceiveInvitationUpdate += RefreshState;
        _signalRService.OnReceiveJoinTableUpdate += RefreshState;
        _signalRService.OnReceiveDeleteTableUpdate += RefreshState;
        _signalRService.OnReceiveEndOfBiddingUpdate += RefreshState;
        _signalRService.OnReceiveStartOfGameUpdate += RefreshState;
        _signalRService.OnReceiveEndOfGameUpdate += RefreshState;

        if (_signalRService.GetConnectionState() != Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
        {
            await _signalRService.StartConnectionAsync();
        }

        await RefreshState();
    }

    private async Task RefreshState()
    {
        var tableId = await GetTableId();
        if (tableId is not null)
        {
            TableId = tableId;
            PlayerState = PlayerState.AT_TABLE;

            var gameId = await GetGameId();
            if (gameId is not null)
            {
                GameId = gameId;
                PlayerState = PlayerState.IN_GAME;
                GamePhase = await GetGamePhase(gameId.Value);
            }
        }
        else
        {
            var inviteTableId = await GetInviteTableId();
            if (inviteTableId is not null)
            {
                InviteTableId = inviteTableId;
                PlayerState = PlayerState.INVITED;
            }
            else
            {
                PlayerState = PlayerState.NONE;
            }
        }

        if (OnStateChanged is not null)
            await OnStateChanged.Invoke();
    }


    private Task<long?> GetInviteTableId() =>
        _apiService.GetSignedInUserInviteTableIdAsync();

    private Task<long?> GetTableId() =>
        _apiService.GetSignedInUserTableIdAsync();

    private Task<long?> GetGameId() =>
        _apiService.GetSignedInUserGameIdAsync();

    private Task<GamePhase> GetGamePhase(long gameId) =>
        _apiService.GetGamePhaseAsync(gameId);
}
