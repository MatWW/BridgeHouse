using Shared.DTOs;

namespace Backend.Services;

public interface IUserStateService
{
    Task<UserStateDTO> GetUserStateAsync();
}
