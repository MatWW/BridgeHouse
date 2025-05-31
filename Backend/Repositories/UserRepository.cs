using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _appDbContext;

    public UserRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public Task<bool> UserExistsAsync(string userId) =>
        _appDbContext.Users.AnyAsync(u => u.Id == userId);

    public Task<string?> GetUserNicknameByIdAsync(string userId) =>
        _appDbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.Nickname)
            .FirstOrDefaultAsync();

    public Task<string?> GetUserIdByNicknameAsync(string nickname) =>
        _appDbContext.Users
            .AsNoTracking()
            .Where(u => u.Nickname == nickname)
            .Select(u => u.Id)
            .FirstOrDefaultAsync();

}
