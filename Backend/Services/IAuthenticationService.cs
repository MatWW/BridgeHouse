using Backend.Data.Models;
using Backend.DTOs;

namespace Backend.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> RegisterUserAsync(RegisterRequestDTO request);
    Task<AuthenticationResponse> AuthenticateUserAsync(LoginRequestDTO request);
    Task<string> RefreshTokenAsync(string refreshToken);
}
