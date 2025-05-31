using Backend.Repositories;
using System.Security.Claims;
using Backend.Exceptions;

namespace Backend.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public UserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public string GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User ?? throw new UnauthorizedAccessException("User is not authorized");

        var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User id claim not found.");

        return id;
    }

    public async Task<string> GetUserIdByNicknameAsync(string nickname)
    {
        var userId = await _userRepository.GetUserIdByNicknameAsync(nickname) 
            ?? throw new UserNotFoundException($"User with nickname: {nickname} was not found");

        return userId;
    }
}
