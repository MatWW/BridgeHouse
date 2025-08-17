using Backend.Data.Models;

namespace Backend.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
    public string ExtractEmail(string token);
}
