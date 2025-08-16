using Shared.Models;

namespace Backend.Services;

public interface IUserStateService
{
    Task<UserStateDTO> GetUserStateAsync();
}
