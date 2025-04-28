using Backend.Repositories;
using Shared;
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

        BridgeTable newBridgeTable = new BridgeTable
        {
            Id = null,
            AdminId = creatorId,
            PlayersIds = new List<string> { creatorId },
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


    public async Task AddUserToBridgeTableAsync(long bridgeTableId, string userId)
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

        List<string>? playersIds = await redisBridgeTableRepository.GetListOfBridgeTablePalyersIdsAsync(bridgeTableId);

        if (playersIds is null)
        {
            throw new BridgeTableNotFoundException($"Bridge table with id: {bridgeTableId} was not found");
        }

        if (playersIds.Contains(userId))
        {
            throw new AddPlayerConflictException($"User with id: {userId} is already assigned to bridge table with id: {bridgeTableId}");
        }

        playersIds.Add(userId);

        await redisBridgeTableRepository.UpdateListOfBridgeTablePlayersIdsAsync(bridgeTableId, playersIds);
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

        List<string>? playersIds = await redisBridgeTableRepository.GetListOfBridgeTablePalyersIdsAsync(bridgeTableId);

        if (playersIds is null)
        {
            throw new BridgeTableNotFoundException($"Bridge table with id: {bridgeTableId} was not found");
        }

        if (!playersIds.Contains(userId))
        {
            throw new RemovePlayerConflictException($"User with id: {userId} is not assigned to bridge table with id: {bridgeTableId}");
        }

        playersIds.Remove(userId);

        await redisBridgeTableRepository.UpdateListOfBridgeTablePlayersIdsAsync(bridgeTableId, playersIds);
    }

    public async Task<bool> ValidateBridgeTableOwnershipAsync(long bridgeTableId)
    {
        string requestSenderId = userService.GetCurrentUserId();
        string? bridgeTableAdminId = await redisBridgeTableRepository.GetTableAdminIdAsync(bridgeTableId);

        return requestSenderId == bridgeTableAdminId;
    }
}
