using Backend.Comparers;
using Backend.Exceptions;
using Backend.Repositories;
using Shared;
using Shared.Enums;

namespace Backend.Services;

public class GameService : IGameService
{
    private readonly IRedisGameStateRepository redisGameStateRepository;
    private readonly IDeckService deckService;
    private readonly Random rnd;
    private readonly IRedisBridgeTableRepository redisBridgeTableRepository;
    private readonly IUserService userService;

    public GameService(IRedisGameStateRepository redisGameStateRepository, IDeckService deckService, Random rnd, 
        IRedisBridgeTableRepository redisBridgeTableRepository, IUserService userService)
    {
        this.redisGameStateRepository = redisGameStateRepository;
        this.deckService = deckService;
        this.rnd = rnd;
        this.redisBridgeTableRepository = redisBridgeTableRepository;
        this.userService = userService;
    }

    public async Task StartGameAsync(long tableId, List<Player> players)
    {
        //await ValidateTableOwnershipAsync(tableId);
        //await ValidateTableAsync(tableId);
        //await ValidatePlayersAsync(tableId, players);

        List<string> playersIds = players.Select(p => p.PlayerId).ToList();

        var gameState = new GameState
        {
            Id = null,
            TableId = tableId,
            Players = players,
            GamePhase = GamePhase.BIDDING,
            PlayerHands = deckService.DealCards(playersIds),
            CurrentPlayerId = playersIds[rnd.Next(playersIds.Count)],
            BiddingState = new BiddingState(),
            PlayingState = new PlayingState()
        };

        long gameId = await redisGameStateRepository.SaveGameStateAsync(gameState);

        await AddInformationAboutPlayersBeingInGameAsync(players, gameId);
    }

    public async Task<GameState> GetGameStateAsync(long gameId)
    {
        var gameState = await redisGameStateRepository.GetGameStateAsync(gameId);

        if (gameState is null)
        {
            throw new GameNotFoundException($"Game with id: {gameId} was not found");
        }

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

    public async Task<Contract> GetContractAsync(long gameId)
    {
        var gameState = await GetGameStateAsync(gameId);

        return gameState.BiddingState.Contract;
    }

    public async Task<Player> GetSignedInPlayerInfoAsync(long gameId)
    {
        string userId = userService.GetCurrentUserId();

        var playerInfo = await GetInfoAboutPlayerInGameAsync(userId, gameId);

        if (playerInfo is null)
        {
            // TODO implement specific exception
            throw new Exception();
        }

        return playerInfo;
    }

    private async Task<Player?> GetInfoAboutPlayerInGameAsync(string playerId,  long gameId)
    {
        var gameState = await redisGameStateRepository.GetGameStateAsync(gameId);

        if (gameState is null)
        {
            throw new GameNotFoundException($"Game with id: {gameId} was not found");
        }

        return gameState.Players.FirstOrDefault(p => p.PlayerId == playerId);
    }

    public async Task<List<Card>> GetPlayerCardsAsync(long gameId, string playerId)
    {
        var gameState = await GetGameStateAsync(gameId);

        string requestSenderId = userService.GetCurrentUserId();

        if (playerId != requestSenderId)
        {
            throw new Exception();
            //TODO implement custom exception
        }

        return gameState.PlayerHands[playerId];
    }

    public async Task<List<Card>> GetDummiesCardsAsync(long gameId)
    {
        var gameState = await GetGameStateAsync(gameId);

        if (gameState.PlayingState.CardPlayActions.Count < 1)
        {
            throw new Exception();
            //TODO implement custom exception
        }

        var dummyId = gameState.PlayingState.Dummy.PlayerId;

        return gameState.PlayerHands[dummyId];
    }

    public async Task<Player> GetPlayerInfoAsync(long gameId, string playerId)
    {
        var playerInfo = await GetInfoAboutPlayerInGameAsync(playerId, gameId);

        if (playerInfo is null)
        {
            // TODO implement specific exception
            throw new Exception();
        }

        return playerInfo;
    }

    public async Task<Player> GetCurrentPlayerInfoAsync(long gameId)
    {
        var gameState = await GetGameStateAsync(gameId);

        var currentPlayerId = gameState.CurrentPlayerId;

        var playerInfo = await GetInfoAboutPlayerInGameAsync(currentPlayerId, gameId);


        if (playerInfo is null)
        {
            // TODO implement specific exception
            throw new Exception();
        }

        return playerInfo;
    }

    private async Task ValidateTableOwnershipAsync(long tableId)
    {
        string requestSenderId = userService.GetCurrentUserId();
        string? tableAdminId = await redisBridgeTableRepository.GetTableAdminIdAsync(tableId);

        if (tableAdminId == null || requestSenderId != tableAdminId)
        {
            throw new BridgeTableOwnershipException($"User sending request is not owner of bridge table {tableId}");
        }
    }

    private async Task ValidateTableAsync(long tableId)
    {
        bool tableExists = await redisBridgeTableRepository.TableExistsAsync(tableId);

        if (!tableExists)
        {
            throw new BridgeTableNotFoundException($"Table with id: {tableId} was not found");
        }

        var dealsIds = await redisBridgeTableRepository.GetDealsIdsAsync(tableId);

        if (dealsIds is null || dealsIds.Count != 0)
        {
            throw new GameAlreadyStartedException($"Game at table with id: {tableId} has already started");
        }
    }

    private async Task ValidatePlayersAsync(long tableId, List<Player> players)
    {
        var tablePlayers = await redisBridgeTableRepository.GetListOfBridgeTablePalyersAsync(tableId);

        if (tablePlayers == null || !tablePlayers.SequenceEqual(players, new PlayerComparer()) || tablePlayers.Count != 4)
        {
            throw new PlayersListNotValidException($"Player list is not valid");
        }
    }

    private async Task AddInformationAboutPlayersBeingInGameAsync(List<Player> players, long gameId)
    {
        foreach (var player in players) 
        {
            await redisGameStateRepository.SaveInformationAboutPlayerBeingInGameAsync(player.PlayerId, gameId);
        }
    }

    public async Task<long?> GetSignedInPlayerGameIdAsync()
    {
        string userId = userService.GetCurrentUserId();

        return await redisGameStateRepository.GetGameIdOfPlayerAsync(userId);
    }
}

