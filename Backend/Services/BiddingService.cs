using Backend.Exceptions;
using Backend.Repositories;
using Shared.Enums;
using Shared;

namespace Backend.Services;

public class BiddingService : IBiddingService
{
    private readonly IRedisGameStateRepository redisGameStateRepository;
    private readonly IUserService userService;

    public BiddingService(IRedisGameStateRepository redisGameStateRepository, IUserService userService)
    {
        this.redisGameStateRepository = redisGameStateRepository;
        this.userService = userService;
    }

    public async Task PlaceBid(long gameId, BidAction bidAction)
    {
        var gameState = await LoadAndValidateGameState(gameId);

        string requestSenderId = userService.GetCurrentUserId();
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
            FinalizeBidding(gameState);
        }
        else
        {
            gameState.CurrentPlayerId = GetNextPlayerId(gameState, bidder);
        }

        await redisGameStateRepository.SaveGameStateAsync(gameState);
    }

    private async Task<GameState> LoadAndValidateGameState(long gameId)
    {
        var gameState = await redisGameStateRepository.GetGameStateAsync(gameId);

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
            throw new UnauthorizedGameActionException($"Only current player may place a bid");

        if (bidAction.Player.PlayerId != senderId)
            throw new UnauthorizedGameActionException($"Player mismatch in bid action");
    }

    private bool IsBidValid(Bid bid, Player bidder, Contract? currentContract)
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
                && IsCurrentContractBiddedByOpponent(bid, bidder, currentContract);
        }

        if (bid.Value == BiddingValue.REDOUBLE)
        {
            return currentContract.IsDoubled && !currentContract.IsRedoubled
                && IsCurrentContractBiddedByOpponent(bid, bidder, currentContract);
        }

        return false;
    }

    private static bool IsBidHigherThanOther(Bid b1, Bid b2)
    {
        return b1.Value > b2.Value || (b1.Value == b2.Value && b1.Suit > b2.Suit);
    }

    private static bool IsCurrentContractBiddedByOpponent(Bid bid, Player bidder, Contract contract)
    {
        return !AreTeammates(bidder.Position, contract.BidAction.Player.Position);
    }

    private static bool AreTeammates(Position a, Position b)
    {
        return (a == Position.N && b == Position.S) ||
            (a == Position.S && b == Position.N) ||
            (a == Position.E && b == Position.W) ||
            (a == Position.W && b == Position.E);
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
        return gameState.BiddingState.BidActions
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

    private static void FinalizeBidding(GameState gameState)
    {
        var finalContract = gameState.BiddingState.Contract;
        if (finalContract is null)
        {
            return;
        }

        var suit = finalContract.BidAction.Bid.Suit;
        var declarer = gameState.BiddingState.BidActions
            .First(b => b.Bid.Suit == suit).Player;

        gameState.CurrentPlayerId = declarer.PlayerId;
        gameState.PlayingState.Declarer = declarer;
        gameState.PlayingState.Dummy = GetDummy(gameState, declarer);
        gameState.GamePhase = GamePhase.PLAYING;
    }
}
