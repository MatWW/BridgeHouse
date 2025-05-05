using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext appDbContext;

    public UserRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<bool> UserExistsAsync(string userId)
    {
        return await appDbContext.Users.AnyAsync(u => u.Id == userId);
    }

    public async Task<string?> GetUserNicknameByIdAsync(string userId)
    {
        var nickname = await appDbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Nickname)
            .FirstOrDefaultAsync();

        return nickname;
    }
}
