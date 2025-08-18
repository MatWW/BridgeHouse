using Backend.DTOs;

namespace Backend.Services;

public interface IUserStateService
{
    Task<UserStateDTO> GetUserStateAsync();
}
