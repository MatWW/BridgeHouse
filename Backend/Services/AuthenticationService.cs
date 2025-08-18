using Backend.Data.Models;
using Backend.Exceptions;
using Backend.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Backend.DTOs;
using System.Security.Authentication;

namespace Backend.Services;

public class AuthenticationService(
    IUserRepository userRepository,
    IJwtService jwtService,
    IPasswordHasher<User> passwordHasher
    ) : IAuthenticationService
{
    public async Task<AuthenticationResponse> RegisterUserAsync(RegisterRequestDTO request)
    {
        var user = new User
        {
            Email = request.Email,
            Nickname = request.Nickname,
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        await userRepository.SaveAsync(user);

        string accessToken = jwtService.GenerateAccessToken(user);
        string refreshToken = jwtService.GenerateRefreshToken(user); 

        return new AuthenticationResponse {  AccessToken = accessToken, RefreshToken = refreshToken };
    }

    public async Task<AuthenticationResponse> AuthenticateUserAsync(LoginRequestDTO request)
    {
        var user = await userRepository.FindByEmailAsync(request.Email)
            ?? throw new UserNotFoundException("user with email " + request.Email + " was not found");

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result != PasswordVerificationResult.Success)
            throw new InvalidCredentialException("invalid password");

        string accessToken = jwtService.GenerateAccessToken(user);
        string refreshToken = jwtService.GenerateRefreshToken(user);

        return new AuthenticationResponse { AccessToken = accessToken, RefreshToken = refreshToken };
    }
    
    public async Task<string> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new SecurityTokenException("Token was not provided");

        string email = jwtService.ExtractEmail(refreshToken);
        var user = await userRepository.FindByEmailAsync(email)
            ?? throw new UserNotFoundException(email);

        string accessToken = jwtService.GenerateAccessToken(user);

        return accessToken;
    }
}
