namespace Backend.Repositories;
public interface IUserRepository
{
    Task<bool> UserExistsAsync(string id);
}
