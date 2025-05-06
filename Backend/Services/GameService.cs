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

    public async Task StartGame(long tableId, List<Player> players)
    {
        await ValidateTableOwnershipAsync(tableId);
        await ValidateTableAsync(tableId);
        await ValidatePlayersAsync(tableId, players);

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

        await redisGameStateRepository.SaveGameStateAsync(gameState);
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

        var dealsIds = await redisBridgeTableRepository.GetDealsIds(tableId);

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
}

