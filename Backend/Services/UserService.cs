using System.Security.Claims;

namespace Backend.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentUserId()
    {
        return httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}
