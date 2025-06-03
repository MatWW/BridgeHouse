using Backend.Exceptions;
using Backend.Repositories;
using Shared;
using Shared.Enums;

namespace Backend.Services;

public class GameService : IGameService
{
    private readonly IRedisGameStateRepository _redisGameStateRepository;
    private readonly IDeckService _deckService;
    private readonly Random _rnd;
    private readonly IRedisBridgeTableRepository _redisBridgeTableRepository;
    private readonly IUserService _userService;
    private readonly IRedisPlayerStateRepository _redisPlayerStateRepository;
    private readonly IGameHistoryService _gameHistoryService;
    private readonly INotificationService _notificationService;

    public GameService(IRedisGameStateRepository redisGameStateRepository, IDeckService deckService, Random rnd, 
        IRedisBridgeTableRepository redisBridgeTableRepository, IUserService userService,
        IRedisPlayerStateRepository redisPlayerStateRepository, IGameHistoryService gameHistoryService,
        INotificationService notificationService)
    {
        _redisGameStateRepository = redisGameStateRepository;
        _deckService = deckService;
        _rnd = rnd;
        _redisBridgeTableRepository = redisBridgeTableRepository;
        _userService = userService;
        _redisPlayerStateRepository = redisPlayerStateRepository;
        _gameHistoryService = gameHistoryService;
        _notificationService = notificationService;
    }

    public async Task StartGameAsync(long tableId, List<Player> players)
    {
        await ValidateTableOwnershipAsync(tableId);
        await ValidateTableAsync(tableId, players);

        List<string> playersIds = players.Select(p => p.PlayerId).ToList();

        var gameState = new GameState
        {
            Id = null,
            TableId = tableId,
            Players = players,
            GamePhase = GamePhase.BIDDING,
            PlayerHands = _deckService.DealCards(playersIds),
            CurrentPlayerId = playersIds[_rnd.Next(playersIds.Count)],
            BiddingState = new BiddingState(),
            PlayingState = new PlayingState()
        };

        long gameId = await _redisGameStateRepository.SaveGameStateAsync(gameState);

        await AddInformationAboutPlayersBeingInGameAsync(players, gameId);

        await _notificationService.SendStartOfGameUpdate(tableId);
    }

    public async Task<GameState> GetGameStateAsync(long gameId)
    {
        var gameState = await _redisGameStateRepository.GetGameStateAsync(gameId)
            ?? throw new GameNotFoundException($"Game with id: {gameId} was not found");

        return gameState;
    }

    public async Task<BiddingState> GetBiddingStateAsync(long gameId)
    {
        var gameState = await GetGameStateAsync(gameId);

        return gameState.BiddingState;
    }
    
    public async Task<PlayingState> GetPlayingStateAsync(long gameId)
    {
        var gameState = await GetGameStateAsync(gameId);

        return gameState.PlayingState;
    }

    public async Task<Contract?> GetContractAsync(long gameId)
    {
        var gameState = await GetGameStateAsync(gameId);

        return gameState.BiddingState.Contract;
    }

    public async Task<Player> GetSignedInPlayerInfoAsync(long gameId)
    {
        string userId = _userService.GetCurrentUserId();

        var playerInfo = await GetInfoAboutPlayerInGameAsync(userId, gameId);

        return playerInfo;
    }

    public async Task<Player> GetInfoAboutPlayerInGameAsync(string playerId,  long gameId)
    {
        var gameState = await _redisGameStateRepository.GetGameStateAsync(gameId);

        if (gameState is null)
        {
            throw new GameNotFoundException($"Game with id: {gameId} was not found");
        }

        return gameState.Players.FirstOrDefault(p => p.PlayerId == playerId)
            ?? throw new PlayerNotFoundInGameException($"Player with id: {playerId} was not found in game with id: {gameId}");
    }

    public async Task<List<Card>> GetSignedInPlayerCardsAsync(long gameId)
    {
        var gameState = await GetGameStateAsync(gameId);

        string playerId = _userService.GetCurrentUserId();

        return gameState.PlayerHands[playerId];
    }

    public async Task<List<Card>> GetDummiesCardsAsync(long gameId)
    {
        var gameState = await GetGameStateAsync(gameId);

        if (gameState.PlayingState.CardPlayActions.Count < 1)
        {
            throw new UnauthorizedAccessException($"Dummies cards from game with id: {gameId} cannot be retrieved before lead");
        }

        var dummyId = gameState.PlayingState.Dummy.PlayerId;

        return gameState.PlayerHands[dummyId];
    }

    public async Task<Player> GetCurrentPlayerInfoAsync(long gameId)
    {
        var gameState = await GetGameStateAsync(gameId);

        var currentPlayerId = gameState.CurrentPlayerId;

        var playerInfo = await GetInfoAboutPlayerInGameAsync(currentPlayerId, gameId);

        return playerInfo;
    }

    public async Task<GamePhase> GetGamePhaseAsync(long gameId)
    {
        var gameState = await GetGameStateAsync(gameId);

        return gameState.GamePhase;
    }

    public async Task EndGameAsync(long gameId)
    {
        var gameState = await GetGameStateAsync(gameId);

        await _redisGameStateRepository.DeleteGameStateAsync(gameId);

        foreach (var player in gameState.Players)
        {
            await _redisPlayerStateRepository.DeleteInformationAboutPlayerBeingInGameAsync(player.PlayerId);
        }

        await _gameHistoryService.SaveGameAsync(gameState);

        await _notificationService.SendEndOfGameUpdate(gameState.TableId);
    }

    private async Task ValidateTableOwnershipAsync(long tableId)
    {
        string requestSenderId = _userService.GetCurrentUserId();
        string? tableAdminId = await _redisBridgeTableRepository.GetTableAdminIdAsync(tableId);

        if (tableAdminId == null || requestSenderId != tableAdminId)
        {
            throw new BridgeTableOwnershipException($"User sending request is not owner of bridge table {tableId}");
        }
    }

    private async Task ValidateTableAsync(long tableId, List<Player> players)
    {
        bool tableExists = await _redisBridgeTableRepository.TableExistsAsync(tableId);

        if (!tableExists)
        {
            throw new BridgeTableNotFoundException($"Table with id: {tableId} was not found");
        }

        if (players.Count != 4)
        {
            throw new PlayersListNotValidException($"Player list is not valid");
        }
    }

    private async Task AddInformationAboutPlayersBeingInGameAsync(List<Player> players, long gameId)
    {
        foreach (var player in players) 
        {
            await _redisPlayerStateRepository.SaveInformationAboutPlayerBeingInGameAsync(player.PlayerId, gameId);
        }
    }
}

