using Backend.Data.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Services;

public class JwtService : IJwtService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessExpirationMinutes;
    private readonly int _refreshExpirationMinutes;
    private readonly TokenValidationParameters _tokenValidationParameters;

    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtService(IConfiguration configuration)
    {
        _secret = configuration["Jwt:Secret"]
                  ?? throw new ArgumentNullException(nameof(configuration), "Jwt:Secret is missing");
        _issuer = configuration["Jwt:Issuer"]
                  ?? throw new ArgumentNullException(nameof(configuration), "Jwt:Issuer is missing");
        _audience = configuration["Jwt:Audience"]
                  ?? throw new ArgumentNullException(nameof(configuration), "Jwt:Audience is missing");
        _accessExpirationMinutes = configuration.GetValue("Jwt:ExpirationInMinutes", 1);
        _refreshExpirationMinutes = configuration.GetValue("Jwt:RefreshToken:ExpirationInMinutes", 60 * 24 * 7);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public string GenerateAccessToken(User user) => GenerateToken(user, _accessExpirationMinutes);

    public string GenerateRefreshToken(User user) => GenerateToken(user, _refreshExpirationMinutes);

    private string GenerateToken(User user, int expirationInMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
            SigningCredentials = creds,
            Issuer = _issuer,
            Audience = _audience
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    public string ExtractEmail(string token)
    {
        var principal = GetPrincipalFromToken(token);

        return principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            ?? throw new SecurityTokenException("Email claim not found in token.");
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        // throws SecurityTokenException if token not valid
        var principal = _tokenHandler.ValidateToken(token, _tokenValidationParameters, out _);

        return principal;
    }
}
