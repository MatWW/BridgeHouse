using Backend.Repositories;
using Shared.Enums;
using Shared.Models;

namespace Backend.Services;

public class UserStateService : IUserStateService
{
    private readonly IRedisPlayerStateRepository _redisPlayerStateRepository;
    private readonly IUserService _userService;

    public UserStateService(IRedisPlayerStateRepository redisPlayerStateRepository, IUserService userService)
    {
        _redisPlayerStateRepository = redisPlayerStateRepository;
        _userService = userService;
    }

    public async Task<UserStateDTO> GetUserStateAsync()
    { 
        long? inviteTableId = await GetSignedInUserInviteTableIdAsync();
        Position? position = await GetSignedInUserInviteTablePositionAsync();

        Invitation? invitation = (inviteTableId != null && position != null)
            ? new Invitation { TableId = inviteTableId.Value, Position = position.Value } 
            : null;

        long? tableId = await GetSignedInUserTableIdAsync();
        long? gameId = await GetSignedInUserGameIdAsync();

        UserStatus status = gameId != null ? UserStatus.IN_GAME
            : tableId != null ? UserStatus.AT_TABLE
            : invitation != null ? UserStatus.INVITED
            : UserStatus.IN_LOBBY;

        return new UserStateDTO
        {
            UserStatus = status,
            Invitation = invitation,
            TableId = tableId,
            GameId = gameId
        };
    }

    private Task<long?> GetSignedInUserTableIdAsync() =>
        _redisPlayerStateRepository.GetTableIdOfPlayerAsync(GetSignedInUserId());
    
    private Task<long?> GetSignedInUserInviteTableIdAsync() =>
        _redisPlayerStateRepository.GetTableIdOfPlayerInviteAsync(GetSignedInUserId());

    private Task<Position?> GetSignedInUserInviteTablePositionAsync() =>
        _redisPlayerStateRepository.GetPositionOfPlayerInviteAsync(GetSignedInUserId());

    private Task<long?> GetSignedInUserGameIdAsync() =>
        _redisPlayerStateRepository.GetGameIdOfPlayerAsync(GetSignedInUserId());

    private string GetSignedInUserId() =>
        _userService.GetCurrentUserId();
    
}
