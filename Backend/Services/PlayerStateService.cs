using Backend.Repositories;

namespace Backend.Services;

public class PlayerStateService : IPlayerStateService
{
    private readonly IRedisPlayerStateRepository _redisPlayerStateRepository;
    private readonly IUserService _userService;

    public PlayerStateService(IRedisPlayerStateRepository redisPlayerStateRepository, IUserService userService)
    {
        _redisPlayerStateRepository = redisPlayerStateRepository;
        _userService = userService;
    }

    public Task<long?> GetSignedInPlayerTableIdAsync() =>
        _redisPlayerStateRepository.GetTableIdOfPlayerAsync(GetSignedInUserId());
    

    public Task<long?> GetSignedInPlayerInviteTableIdAsync() =>
        _redisPlayerStateRepository.GetTableIdOfPlayerInviteAsync(GetSignedInUserId());
    

    public Task<long?> GetSignedInPlayerGameIdAsync() =>
        _redisPlayerStateRepository.GetGameIdOfPlayerAsync(GetSignedInUserId());

    private string GetSignedInUserId()
    {
        return _userService.GetCurrentUserId();
    }
}
