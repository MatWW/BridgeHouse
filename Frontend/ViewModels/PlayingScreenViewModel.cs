using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Enums;
using Shared.Models;

namespace Frontend.ViewModels;

public class PlayingViewModel
{
    private readonly IApiService _apiService;
    private readonly ISignalRService _signalRService;

    public long GameId { get; }
    public long TableId { get; }

    public List<Card?> CardsToDisplay { get; private set; } = new() { null, null, null, null };
    public Player SignedInPlayer { get; private set; } = new();
    public List<Card> PlayerCards { get; private set; } = new();
    public PlayingState PlayingState { get; private set; } = new();
    public Player CurrentTurnPlayer { get; private set; } = new();
    public Contract Contract { get; private set; } = new();
    public Player Dummy { get; private set; } = new();
    public List<Card> DummiesCards { get; private set; } = new();
    public bool CanShowDummy { get; private set; } = false;

    public event Func<Task>? OnStateChanged;

    public PlayingViewModel(long gameId, long tableId, IApiService apiService, ISignalRService signalRService)
    {
        GameId = gameId;
        TableId = tableId;
        _apiService = apiService;
        _signalRService = signalRService;
    }

    public async Task InitializeAsync()
    {
        await LoadStateAsync();

        if (_signalRService.GetConnectionState() != HubConnectionState.Connected)
        {
            await _signalRService.StartConnectionAsync();
        }

        await _signalRService.JoinTableAsync(TableId);
        _signalRService.OnReceiveCardPlayUpdate += UpdateStateAsync;
    }

    private async Task LoadStateAsync()
    {
        PlayingState = await _apiService.GetPlayingStateAsync(GameId);
        SignedInPlayer = await _apiService.GetSignedInPlayerInfoAsync(GameId);
        CurrentTurnPlayer = await _apiService.GetCurrentTurnPlayerInfoAsync(GameId);
        PlayerCards = await _apiService.GetSignedInPlayerCardsAsync(GameId);
        CardsToDisplay = GetCardsToDisplay(PlayingState.CardsOnTable);

        if (PlayingState.CardPlayActions.Count >= 1)
        {
            CanShowDummy = true;
            Dummy = PlayingState.Dummy;
            DummiesCards = await _apiService.GetDummiesCardsAsync(GameId);
        }

        Contract = await _apiService.GetContractAsync(GameId);

        if (OnStateChanged is not null)
            await OnStateChanged.Invoke();
    }

    private async Task UpdateStateAsync()
    {
        await LoadStateAsync();
    }

    public async Task HandleCardClick(Card card)
    {
        var action = new CardPlayAction
        {
            CardPlayed = card,
            Player = CurrentTurnPlayer
        };

        await _apiService.PlayCardAsync(GameId, action);
    }

    public int GetIndexForPosition(Position position, Position playerPosition)
    {
        Position[] clockwise = new[] { Position.W, Position.N, Position.E, Position.S };
        int playerIndex = Array.IndexOf(clockwise, playerPosition);
        int targetIndex = Array.IndexOf(clockwise, position);
        return (targetIndex - playerIndex + 4) % 4;
    }

    private List<Card?> GetCardsToDisplay(List<CardPlayAction> actions)
    {
        var result = new List<Card?> { null, null, null, null };
        foreach (var action in actions)
        {
            var index = GetIndexForPosition(action.Player.Position, SignedInPlayer.Position);
            result[index] = action.CardPlayed;
        }
        return result;
    }
}
