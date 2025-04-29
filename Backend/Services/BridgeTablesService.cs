using Backend.Repositories;
using Shared;
using Shared.Enums;
using Backend.Exceptions;

namespace Backend.Services;

public class BridgeTablesService : IBridgeTablesService
{
    private readonly IRedisBridgeTableRepository redisBridgeTableRepository;
    private readonly IUserService userService;
    private readonly IUserRepository userRepository;

    public BridgeTablesService(IRedisBridgeTableRepository redisTableRepository, IUserService userService, IUserRepository userRepository)
    {
        this.redisBridgeTableRepository = redisTableRepository;
        this.userService = userService;
        this.userRepository = userRepository;
    }

    public async Task<List<BridgeTable>> GetAllBridgeTablesAsync()
    {
        return await redisBridgeTableRepository.GetAllBridgeTablesAsync();
    }

    public async Task<BridgeTable> GetBridgeTableByIdAsync(long bridgeTableId)
    {
        BridgeTable? table = await redisBridgeTableRepository.GetBridgeTableByIdAsync(bridgeTableId);

        if (table is null)
        {
            throw new BridgeTableNotFoundException($"Bridge table with id: {bridgeTableId} was not found");
        }

        return table;
    }

    public async Task<BridgeTable> CreateBridgeTableAsync(int numberOfDeals)
    {
        string creatorId = userService.GetCurrentUserId();
        var creator = new Player
        {
            PlayerId = creatorId,
            // TODO get real username
            UserName = "username",
            Position = Position.N
        };


        BridgeTable newBridgeTable = new BridgeTable
        {
            Id = null,
            AdminId = creatorId,
            Players = new List<Player> { creator },
            NumberOfDeals = numberOfDeals
        };

        return await redisBridgeTableRepository.SaveBridgeTableAsync(newBridgeTable);
    }

    public async Task DeleteBridgeTableAsync(long bridgeTableId)
    {
        bool requestSenderValid = await ValidateBridgeTableOwnershipAsync(bridgeTableId);

        if (requestSenderValid)
        {
            throw new BridgeTableOwnershipException($"User sending request is not owner of bridge table {bridgeTableId}");
        }

        bool deleted = await redisBridgeTableRepository.DeleteBridgeTableAsync(bridgeTableId);

        if (!deleted)
        {
            throw new BridgeTableNotFoundException($"Bridge table with id: {bridgeTableId} was not found");
        }
    }

    public async Task AddUserToBridgeTableAsync(long bridgeTableId, string userId, Position position)
    {
        bool requestSenderValid = await ValidateBridgeTableOwnershipAsync(bridgeTableId);

        if (requestSenderValid)
        {
            throw new BridgeTableOwnershipException($"User sending request is not owner of bridge table {bridgeTableId}");
        }

        bool userExists = await userRepository.UserExistsAsync(userId);

        if (!userExists)
        {
            throw new UserDoesNotExistException($"User with id: {userId} does not exist");
        }

        List<Player>? players = await redisBridgeTableRepository.GetListOfBridgeTablePalyersAsync(bridgeTableId);

        if (players is null)
        {
            throw new BridgeTableNotFoundException($"Bridge table with id: {bridgeTableId} was not found");
        }

        var player = new Player
        {
            PlayerId = userId,
            // TODO get real username
            UserName = "username",
            Position = position
        };

        if (players.Contains(player))
        {
            throw new AddPlayerConflictException($"User with id: {userId} is already assigned to bridge table with id: {bridgeTableId}");
        }

        players.Add(player);

        await redisBridgeTableRepository.UpdateListOfBridgeTablePlayersAsync(bridgeTableId, players);
    }

    public async Task RemoveUserFromBridgeTableAsync(long bridgeTableId, string userId)
    {
        bool requestSenderValid = await ValidateBridgeTableOwnershipAsync(bridgeTableId);

        if (requestSenderValid)
        {
            throw new BridgeTableOwnershipException($"User sending request is not owner of bridge table {bridgeTableId}");
        }

        bool userExists = await userRepository.UserExistsAsync(userId);

        if (!userExists)
        {
            throw new UserDoesNotExistException($"User with id: {userId} does not exist");
        }

        List<Player>? players = await redisBridgeTableRepository.GetListOfBridgeTablePalyersAsync(bridgeTableId);

        if (players is null)
        {
            throw new BridgeTableNotFoundException($"Bridge table with id: {bridgeTableId} was not found");
        }

        var player = players.FirstOrDefault(p => p.PlayerId == userId);

        if (player is null)
        {
            throw new RemovePlayerConflictException($"User with id: {userId} is not assigned to bridge table with id: {bridgeTableId}");
        }

        players.Remove(player);

        await redisBridgeTableRepository.UpdateListOfBridgeTablePlayersAsync(bridgeTableId, players);
    }

    public async Task<bool> ValidateBridgeTableOwnershipAsync(long bridgeTableId)
    {
        string requestSenderId = userService.GetCurrentUserId();
        string? bridgeTableAdminId = await redisBridgeTableRepository.GetTableAdminIdAsync(bridgeTableId);

        return requestSenderId == bridgeTableAdminId;
    }
}
