using Backend.Repositories;
using Shared;
using Shared.Enums;
using Backend.Exceptions;

namespace Backend.Services;

public class BridgeTablesService : IBridgeTablesService
{
    private readonly IRedisBridgeTableRepository _redisBridgeTableRepository;
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly IRedisPlayerStateRepository _redisPlayerStateRepository;

    public BridgeTablesService(IRedisBridgeTableRepository redisTableRepository, IUserService userService, IUserRepository userRepository,
        IRedisPlayerStateRepository redisPlayerStateRepository)
    {
        _redisBridgeTableRepository = redisTableRepository;
        _userService = userService;
        _userRepository = userRepository;
        _redisPlayerStateRepository = redisPlayerStateRepository;
    }

    public async Task<List<BridgeTable>> GetAllBridgeTablesAsync()
    {
        return await _redisBridgeTableRepository.GetAllBridgeTablesAsync();
    }

    public async Task<BridgeTable> GetBridgeTableByIdAsync(long bridgeTableId)
    {
        BridgeTable? table = await _redisBridgeTableRepository.GetBridgeTableByIdAsync(bridgeTableId);

        if (table is null)
        {
            throw new BridgeTableNotFoundException($"Bridge table with id: {bridgeTableId} was not found");
        }

        return table;
    }

    public async Task<BridgeTable> CreateBridgeTableAsync(CreateBridgeTableRequestDTO request)
    {
        string creatorId = _userService.GetCurrentUserId();
        var creator = new Player
        {
            PlayerId = creatorId,
            Nickname = await _userRepository.GetUserNicknameByIdAsync(creatorId) ?? "",
            Position = Position.N
        };


        BridgeTable newBridgeTable = new BridgeTable
        {
            Id = null,
            AdminId = creatorId,
            Players = new List<Player> { creator },
            NumberOfDeals = request.NumberOfDeals,
            DealsIds = []
        };

        var createdTable = await _redisBridgeTableRepository.SaveBridgeTableAsync(newBridgeTable);

        await _redisPlayerStateRepository.SaveInformationAboutPlayerBeingPartOfTableAsync(creatorId, createdTable.Id!.Value);

        return createdTable;
    }

    public async Task DeleteBridgeTableAsync(long bridgeTableId)
    {
        bool requestSenderValid = await ValidateBridgeTableOwnershipAsync(bridgeTableId);

        if (!requestSenderValid)
        {
            throw new BridgeTableOwnershipException($"User sending request is not owner of bridge table {bridgeTableId}");
        }

        List<Player>? players = await _redisBridgeTableRepository.GetListOfBridgeTablePalyersAsync(bridgeTableId);

        if (players is null)
        {
            throw new BridgeTableNotFoundException($"Bridge table with id: {bridgeTableId} was not found");
        }

        foreach (var player in players)
        {
            await _redisPlayerStateRepository.DeleteInformationAboutPlayerBeingPartOfTableAsync(player.PlayerId);
        }

        bool deleted = await _redisBridgeTableRepository.DeleteBridgeTableAsync(bridgeTableId);

        if (!deleted)
        {
            throw new BridgeTableNotFoundException($"Bridge table with id: {bridgeTableId} was not found");
        }
    }

    public async Task AddUserToBridgeTableAsync(long bridgeTableId, string userId, Position position)
    {
        bool userExists = await _userRepository.UserExistsAsync(userId);

        if (!userExists)
        {
            throw new UserNotFoundException($"User with id: {userId} does not exist");
        }

        List<Player>? players = await _redisBridgeTableRepository.GetListOfBridgeTablePalyersAsync(bridgeTableId);

        if (players is null)
        {
            throw new BridgeTableNotFoundException($"Bridge table with id: {bridgeTableId} was not found");
        }

        var player = new Player
        {
            PlayerId = userId,
            Nickname = await _userRepository.GetUserNicknameByIdAsync(userId) ?? "",
            Position = position
        };

        if (players.Contains(player))
        {
            throw new AddPlayerConflictException($"User with id: {userId} is already assigned to bridge table with id: {bridgeTableId}");
        }

        players.Add(player);

        await _redisBridgeTableRepository.UpdateListOfBridgeTablePlayersAsync(bridgeTableId, players);

        await _redisPlayerStateRepository.SaveInformationAboutPlayerBeingPartOfTableAsync(userId, bridgeTableId);
    }

    public async Task RemoveUserFromBridgeTableAsync(long bridgeTableId, string userId)
    {
        bool requestSenderValid = await ValidateBridgeTableOwnershipAsync(bridgeTableId);

        if (!requestSenderValid && _userService.GetCurrentUserId() != userId)
        {
            throw new BridgeTableOwnershipException($"User sending request is not owner of bridge table {bridgeTableId}");
        }

        bool userExists = await _userRepository.UserExistsAsync(userId);

        if (!userExists)
        {
            throw new UserNotFoundException($"User with id: {userId} does not exist");
        }

        List<Player>? players = await _redisBridgeTableRepository.GetListOfBridgeTablePalyersAsync(bridgeTableId);

        if (players is null)
        {
            throw new BridgeTableNotFoundException($"Bridge table with id: {bridgeTableId} was not found");
        }

        var player = players.FirstOrDefault(p => p.PlayerId == userId);

        if (player is null)
        {
            throw new PlayerNotFoundAtBridgeTableException($"User with id: {userId} is not assigned to bridge table with id: {bridgeTableId}");
        }

        players.Remove(player);

        await _redisBridgeTableRepository.UpdateListOfBridgeTablePlayersAsync(bridgeTableId, players);

        await _redisPlayerStateRepository.DeleteInformationAboutPlayerBeingPartOfTableAsync(userId);
    }

    public async Task InviteUserToBridgeTableAsync(long bridgeTableId, string userId, Position position)
    {
        await _redisPlayerStateRepository.SaveInformationAboutPlayerBeingInvitedToTableAsync(userId, bridgeTableId, position);
    }

    public async Task AcceptInviteToBridgeTableAsync(long bridgeTableId, string userId)
    {
        Position? position = await _redisPlayerStateRepository.GetPositionOfPlayerInviteAsync(userId);

        if (position is null)
        {
            throw new InviteNotFoundException($"Valid invite of user with id: {userId} was not found");
        }

        await _redisPlayerStateRepository.DeleteInformationAboutPlayerBeingInvitedToTableAsync(userId);

        await AddUserToBridgeTableAsync(bridgeTableId, userId, position.Value);
    }

    public async Task DeclineInviteToBridgeTableAsync(string userId)
    {
        await _redisPlayerStateRepository.DeleteInformationAboutPlayerBeingInvitedToTableAsync(userId);
    }

    public async Task<bool> ValidateBridgeTableOwnershipAsync(long bridgeTableId)
    {
        string requestSenderId = _userService.GetCurrentUserId();
        string? bridgeTableAdminId = await _redisBridgeTableRepository.GetTableAdminIdAsync(bridgeTableId);

        return requestSenderId == bridgeTableAdminId;
    }
}
