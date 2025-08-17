using Backend.Data.Models;

namespace Backend.Repositories;
public interface IUserRepository
{
    Task<bool> UserExistsAsync(string id);
    Task<string?> GetUserNicknameByIdAsync(string userId);
    Task<string?> GetUserIdByNicknameAsync(string nickname);
    Task<User?> FindByEmailAsync(string email);
    Task SaveAsync(User user);
}
