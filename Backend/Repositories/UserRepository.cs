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

    public Task<bool> UserExistsAsync(string userId) =>
        appDbContext.Users.AnyAsync(u => u.Id == userId);

    public Task<string?> GetUserNicknameByIdAsync(string userId) =>
        appDbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.Nickname)
            .FirstOrDefaultAsync();

        

    public Task<string?> GetUserIdByNicknameAsync(string nickname) =>
        appDbContext.Users
            .AsNoTracking()
            .Where(u => u.Nickname == nickname)
            .Select(u => u.Id)
            .FirstOrDefaultAsync();

}
