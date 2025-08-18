using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.DTOs;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
    {
        private static CookieOptions BuildCookieOptions(int minutes) => new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(minutes)
        };

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
        {
            var tokens = await authenticationService.RegisterUserAsync(dto);

            Response.Cookies.Append("access-token", tokens.AccessToken, BuildCookieOptions(1));
            Response.Cookies.Append("refresh-token", tokens.RefreshToken, BuildCookieOptions(10080));

            return CreatedAtAction(nameof(Register), dto.Email);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            var tokens = await authenticationService.AuthenticateUserAsync(dto);

            Response.Cookies.Append("access-token", tokens.AccessToken, BuildCookieOptions(1));
            Response.Cookies.Append("refresh-token", tokens.RefreshToken, BuildCookieOptions(10080));

            return Ok(new {dto.Email});
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue("refresh-token", out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized("Refresh token missing.");

            var accessToken = await authenticationService.RefreshTokenAsync(refreshToken);

            Response.Cookies.Append("access-token", accessToken, BuildCookieOptions(1));

            return Ok();
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access-token");
            Response.Cookies.Delete("refresh-token");
            
            return NoContent();
        }
    }
}
