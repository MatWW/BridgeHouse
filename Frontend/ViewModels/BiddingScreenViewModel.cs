using Shared;
using Shared.Enums;
using Frontend.Services.Interfaces;

namespace Frontend.ViewModels;

public class BiddingViewModel
{
    private readonly IApiService _apiService;
    private readonly ISignalRService _signalRService;

    public BiddingState? BiddingState { get; private set; }
    public Contract? Contract => BiddingState?.Contract;
    public Player? SignedInPlayer { get; private set; }
    public Player? CurrentTurnPlayer { get; private set; }
    public List<Card> PlayerCards { get; private set; } = new();

    public long GameId { get; }
    public long TableId { get; }

    public event Func<Task>? OnStateChanged;

    public BiddingViewModel(long gameId, long tableId, IApiService apiService, ISignalRService signalR)
    {
        GameId = gameId;
        TableId = tableId;
        _apiService = apiService;
        _signalRService = signalR;
    }

    public async Task InitializeAsync()
    {
        await LoadStateAsync();

        if (_signalRService.GetConnectionState() != Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
        {
            await _signalRService.StartConnectionAsync();
        }

        await _signalRService.JoinTableAsync(TableId);

        _signalRService.OnReceiveBidUpdate += UpdateAsync;
    }

    private async Task LoadStateAsync()
    {
        BiddingState = await _apiService.GetBiddingStateAsync(GameId);
        SignedInPlayer = await _apiService.GetSignedInPlayerInfoAsync(GameId);
        CurrentTurnPlayer = await _apiService.GetCurrentTurnPlayerInfoAsync(GameId);
        PlayerCards = await _apiService.GetSignedInPlayerCardsAsync(GameId);

        if (OnStateChanged is not null)
            await OnStateChanged.Invoke();
    }

    private async Task UpdateAsync()
    {
        BiddingState = await _apiService.GetBiddingStateAsync(GameId);
        CurrentTurnPlayer = await _apiService.GetCurrentTurnPlayerInfoAsync(GameId);

        if (OnStateChanged is not null)
            await OnStateChanged.Invoke();
    }

    public async Task PlaceBidAsync(BiddingSuit suit, BiddingValue value)
    {
        if (SignedInPlayer == null) return;

        var action = new BidAction
        {
            Player = SignedInPlayer,
            Bid = new Bid { Suit = suit, Value = value }
        };

        await _apiService.PlaceBidAsync(GameId, action);
    }

    public bool IsSignedInPlayerTurn() => SignedInPlayer?.PlayerId == CurrentTurnPlayer?.PlayerId;

    public bool CanDouble()
    {
        return Contract != null && !Contract.IsDoubled && !Contract.IsRedoubled &&
               IsOpponent(Contract.BidAction.Player.Position);
    }

    public bool CanRedouble()
    {
        return Contract != null && Contract.IsDoubled && !Contract.IsRedoubled &&
               !IsOpponent(Contract.BidAction.Player.Position);
    }

    public BiddingValue GetLowestAllowedValue(BiddingSuit suit)
    {
        if (Contract == null) return BiddingValue.ONE;

        var current = Contract.BidAction.Bid;
        return current.Suit >= suit ? current.Value + 1 : current.Value;
    }

    public int GetStartingPositionOffset()
    {
        var bids = BiddingState?.BidActions;
        return bids.Count == 0 ? (int)(CurrentTurnPlayer?.Position) : (int)bids[0].Player.Position;
    }

    private bool IsOpponent(Position other)
    {
        var self = SignedInPlayer?.Position;
        return !AreTeammates(self.Value, other);
    }

    private static bool AreTeammates(Position a, Position b) =>
        (a == Position.N && b == Position.S) || (a == Position.S && b == Position.N) ||
        (a == Position.E && b == Position.W) || (a == Position.W && b == Position.E) || 
        (a == b);
}
