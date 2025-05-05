namespace Backend.Repositories;
public interface IUserRepository
{
    Task<bool> UserExistsAsync(string id);
    Task<string?> GetUserNicknameByIdAsync(string userId);
}
