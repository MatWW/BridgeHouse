using Backend.Exceptions;
using Backend.Repositories;
using Backend.Enums;
using Backend.Enums.Extensions;
using Backend.Models;

namespace Backend.Services;

public class PlayingService : IPlayingService
{
    private readonly IRedisGameStateRepository _redisGameStateRepository;
    private readonly IUserService _userService;
    private readonly IGameService _gameService;
    private readonly INotificationService _notificationService;

    public PlayingService(IRedisGameStateRepository redisGameStateRepository, IUserService userService, IGameService gameService
        , INotificationService notificationService)
    {
        _redisGameStateRepository = redisGameStateRepository;
        _userService = userService;
        _gameService = gameService;
        _notificationService = notificationService;
    }

    public async Task PlayCardAsync(long gameId, CardPlayAction cardPlayAction)
    {
        var gameState = await LoadAndValidateGameState(gameId);

        string requestSenderId = _userService.GetCurrentUserId();
        ValidatePlayerPermissions(gameState, cardPlayAction, requestSenderId);

        if (!IsPlayValid(gameState, cardPlayAction))
        {
            throw new IllegalCardPlayException("This card play is not valid");
        }

        ApplyCardPlay(gameState, cardPlayAction);

        await _redisGameStateRepository.SaveGameStateAsync(gameState);

        await _notificationService.SendCardPlayUpdate(gameState.TableId);

        if (AllTricksPlayed(gameState.PlayingState))
        {
            await _gameService.EndGameAsync(gameId);
        }
    }

    private async Task<GameState> LoadAndValidateGameState(long gameId)
    {
        var gameState = await _redisGameStateRepository.GetGameStateAsync(gameId);

        if (gameState is null)
        {
            throw new GameNotFoundException($"Game with id: {gameId} was not found");
        }

        if (gameState.GamePhase != GamePhase.PLAYING)
        {
            throw new GamePhaseException("Game is not in playing phase");
        }

        return gameState;
    }

    private static void ValidatePlayerPermissions(GameState gameState, CardPlayAction cardPlayAction, string senderId)
    {
        if (gameState.CurrentPlayerId != gameState.PlayingState.Dummy.PlayerId)
        {
            if (gameState.CurrentPlayerId != senderId)
                throw new UnauthorizedGameActionException($"Only current player can play a card");

            if (cardPlayAction.Player.PlayerId != senderId)
                throw new UnauthorizedGameActionException($"Player mismatch in card play action");
        }
        else
        {
            if (gameState.PlayingState.Declarer.PlayerId != senderId)
                throw new UnauthorizedGameActionException($"Only current player can play a card");

            if (cardPlayAction.Player.PlayerId != gameState.PlayingState.Dummy.PlayerId)
                throw new UnauthorizedGameActionException($"Player mismatch in card play action");
        }
    }

    private static bool IsPlayValid(GameState gameState, CardPlayAction cardPlayAction)
    {
        var playingState = gameState.PlayingState;

        var playedCard = cardPlayAction.CardPlayed;
        var currentPlayerHand = gameState.PlayerHands[gameState.CurrentPlayerId];

        if (!IsCardInPlayersHand(currentPlayerHand, playedCard))
        {
            return false;
        }

        var cardsOnTable = playingState.CardsOnTable;

        if (cardsOnTable.Count == 0 || cardsOnTable.Count == 4)
        {
            return true;
        }

        if (cardsOnTable[0].CardPlayed.Suit != playedCard.Suit)
        {
            return !HandContainesCardInSuit(currentPlayerHand, cardsOnTable[0].CardPlayed);
        }

        return true;
    }

    private static bool IsCardInPlayersHand(List<Card> playerCards, Card card)
    {
        return playerCards.Any(c => c.Suit == card.Suit && c.Value == card.Value);
    }

    private static bool HandContainesCardInSuit(List<Card> hand, Card card)
    {
        return hand.Any(c => c.Suit == card.Suit);
    }

    private static void ApplyCardPlay(GameState gameState, CardPlayAction cardPlayAction)
    {
        gameState.PlayingState.CardPlayActions.Add(cardPlayAction);

        var playerHand = gameState.PlayerHands[gameState.CurrentPlayerId];
        playerHand.RemoveAll(c => c.Suit == cardPlayAction.CardPlayed.Suit && c.Value == cardPlayAction.CardPlayed.Value);

        var cardsOnTable = gameState.PlayingState.CardsOnTable;

        if (cardsOnTable.Count == 4)
        {
            gameState.PlayingState.CardsOnTable = [];
        }

        gameState.PlayingState.CardsOnTable.Add(cardPlayAction);

        if (gameState.PlayingState.CardsOnTable.Count < 4)
        {
            gameState.CurrentPlayerId = GetNextPlayerId(gameState, cardPlayAction.Player);
        }
        else
        {
            FinalizeTrick(gameState, cardsOnTable);
        }
    }

    private static void FinalizeTrick(GameState gameState, List<CardPlayAction> cardsOnTable)
    {
        var contractSuit = gameState.BiddingState.Contract!.BidAction.Bid.Suit;

        CardPlayAction? winningCard;

        if (contractSuit == BiddingSuit.NO_TRUMP)
        {
            var winningSuit = cardsOnTable[0].CardPlayed.Suit;

            winningCard = cardsOnTable
                .Where(c => c.CardPlayed.Suit == winningSuit)
                .MaxBy(c => c.CardPlayed.Value);
        }
        else
        {
            var trumpSuit = contractSuit.ToCardSuit();

            winningCard = cardsOnTable
                .Where(c => c.CardPlayed.Suit == trumpSuit)
                .MaxBy(c => c.CardPlayed.Value);

            if (winningCard is null)
            {
                var winningSuit = cardsOnTable[0].CardPlayed.Suit;

                winningCard = cardsOnTable
                    .Where(c => c.CardPlayed.Suit == winningSuit)
                    .MaxBy(c => c.CardPlayed.Value);
            }
        }

        var trickWinner = winningCard!.Player;
        gameState.CurrentPlayerId = trickWinner.PlayerId;

        if (trickWinner.Position == Position.N || trickWinner.Position == Position.S)
        {
            gameState.PlayingState.NSTricks += 1;
        }
        else
        {
            gameState.PlayingState.EWTricks += 1;
        }
    }

    private static readonly Dictionary<Position, Position> NextClockwise = new()
    {
        [Position.N] = Position.E,
        [Position.E] = Position.S,
        [Position.S] = Position.W,
        [Position.W] = Position.N
    };

    private static string GetNextPlayerId(GameState gameState, Player player)
    {
        var nextPosition = NextClockwise[player.Position];
        return gameState.Players.First(p => p.Position == nextPosition).PlayerId;
    }

    private static bool AllTricksPlayed(PlayingState state) =>
        state.NSTricks + state.EWTricks >= 13;
}
