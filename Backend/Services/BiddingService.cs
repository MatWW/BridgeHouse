using Backend.Exceptions;
using Backend.Repositories;
using Backend.Enums;
using Backend.Models;

namespace Backend.Services;

public class BiddingService : IBiddingService
{
    private readonly IRedisGameStateRepository _redisGameStateRepository;
    private readonly IUserService _userService;
    private readonly IGameService _gameService;
    private readonly INotificationService _notificationService;

    public BiddingService(IRedisGameStateRepository redisGameStateRepository, IUserService userService, IGameService gameService,
        INotificationService notificationService)
    {
        _redisGameStateRepository = redisGameStateRepository;
        _userService = userService;
        _gameService = gameService;
        _notificationService = notificationService;
    }

    public async Task PlaceBidAsync(long gameId, BidAction bidAction)
    {
        var gameState = await LoadAndValidateGameState(gameId);

        string requestSenderId = _userService.GetCurrentUserId();
        ValidatePlayerPermissions(gameState, bidAction, requestSenderId);

        var bid = bidAction.Bid;
        var bidder = bidAction.Player;
        var currentContract = gameState.BiddingState.Contract;

        if (!IsBidValid(bid, bidder, currentContract))
        {
            throw new IllegalBidException("This bid is illegal");
        }

        UpdateBiddingState(gameState, bidAction);

        if (IsBiddingOver(gameState))
        {
            var finalContract = gameState.BiddingState.Contract;
            if (finalContract is null)
            {
                await _gameService.EndGameAsync(gameState.Id!.Value);
                return;
            }

            await FinalizeBidding(gameState);
        }
        else
        {
            gameState.CurrentPlayerId = GetNextPlayerId(gameState, bidder);
        }

        await _redisGameStateRepository.SaveGameStateAsync(gameState);

        await _notificationService.SendBidUpdate(gameState.TableId);
    }

    private async Task<GameState> LoadAndValidateGameState(long gameId)
    {
        var gameState = await _redisGameStateRepository.GetGameStateAsync(gameId);

        if (gameState is null)
        {
            throw new GameNotFoundException($"Game with id: {gameId} was not found");
        }

        if (gameState.GamePhase != GamePhase.BIDDING)
        {
            throw new GamePhaseException("Game is not in bidding phase");
        }

        return gameState;
    }

    private static void ValidatePlayerPermissions(GameState gameState, BidAction bidAction, string senderId)
    {
        if (gameState.CurrentPlayerId != senderId)
            throw new UnauthorizedGameActionException($"Only current player can place a bid");

        if (bidAction.Player.PlayerId != senderId)
            throw new UnauthorizedGameActionException($"Player mismatch in bid action");
    }

    private static bool IsBidValid(Bid bid, Player bidder, Contract? currentContract)
    {
        if (bid.Value == BiddingValue.PASS)
        {
            return true;
        }

        if (currentContract is null)
        {
            return bid.Suit != BiddingSuit.NONE;
        }

        if (bid.Suit != BiddingSuit.NONE)
        {
            return IsBidHigherThanOther(bid, currentContract.BidAction.Bid);
        }

        if (bid.Value == BiddingValue.DOUBLE)
        {
            return !currentContract.IsDoubled && !currentContract.IsRedoubled
                && IsCurrentContractBiddedByOpponent(bidder, currentContract);
        }

        if (bid.Value == BiddingValue.REDOUBLE)
        {
            return currentContract.IsDoubled && !currentContract.IsRedoubled
                && !IsCurrentContractBiddedByOpponent(bidder, currentContract);
        }

        return false;
    }

    private static bool IsBidHigherThanOther(Bid b1, Bid b2)
    {
        return b1.Value > b2.Value || (b1.Value == b2.Value && b1.Suit > b2.Suit);
    }

    private static bool IsCurrentContractBiddedByOpponent(Player bidder, Contract contract)
    {
        return !AreTeammates(bidder.Position, contract.BidAction.Player.Position);
    }

    private static bool AreTeammates(Position a, Position b)
    {
        return (a == Position.N && b == Position.S) ||
            (a == Position.S && b == Position.N) ||
            (a == Position.E && b == Position.W) ||
            (a == Position.W && b == Position.E) ||
            (a == b);
    }

    private static void UpdateBiddingState(GameState gameState, BidAction bidAction)
    {
        gameState.BiddingState.BidActions.Add(bidAction);

        if (bidAction.Bid.Suit != BiddingSuit.NONE)
        {
            gameState.BiddingState.Contract = new Contract
            {
                BidAction = bidAction,
                IsDoubled = false,
                IsRedoubled = false
            };
        }
        else if (bidAction.Bid.Value == BiddingValue.DOUBLE)
        {
            gameState.BiddingState.Contract!.IsDoubled = true;
        }
        else if (bidAction.Bid.Value == BiddingValue.REDOUBLE)
        {
            gameState.BiddingState.Contract!.IsRedoubled = true;
        }
    }

    private static bool IsBiddingOver(GameState gameState)
    {
        var bidActions = gameState.BiddingState.BidActions;

        return bidActions.Count > 3 && bidActions
            .TakeLast(3)
            .All(b => b.Bid.Value == BiddingValue.PASS);
    }

    private static readonly Dictionary<Position, Position> NextClockwise = new()
    {
        [Position.N] = Position.E,
        [Position.E] = Position.S,
        [Position.S] = Position.W,
        [Position.W] = Position.N
    };

    private static string GetNextPlayerId(GameState gameState, Player bidder)
    {
        var nextPosition = NextClockwise[bidder.Position];
        return gameState.Players.First(p => p.Position == nextPosition).PlayerId;
    }

    private static readonly Dictionary<Position, Position> Opposite = new()
    {
        [Position.N] = Position.S,
        [Position.S] = Position.N,
        [Position.E] = Position.W,
        [Position.W] = Position.E
    };

    private static Player GetDummy(GameState gameState, Player declarer)
    {
        var dummyPosition = Opposite[declarer.Position];
        return gameState.Players.First(p => p.Position == dummyPosition);
    }

    private async Task FinalizeBidding(GameState gameState)
    { 
        var suit = gameState.BiddingState.Contract!.BidAction.Bid.Suit;
        var declarer = gameState.BiddingState.BidActions
            .First(b => b.Bid.Suit == suit).Player;

        gameState.CurrentPlayerId = GetNextPlayerId(gameState, declarer);
        gameState.PlayingState.Declarer = declarer;
        gameState.PlayingState.Dummy = GetDummy(gameState, declarer);
        gameState.GamePhase = GamePhase.PLAYING;

        await _notificationService.SendEndOfBiddingUpdate(gameState.TableId);
    }
}
