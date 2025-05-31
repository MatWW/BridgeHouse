namespace Backend.Services;

public interface IUserService
{
    string GetCurrentUserId();
    Task<string> GetUserIdByNicknameAsync(string nickname);
}
