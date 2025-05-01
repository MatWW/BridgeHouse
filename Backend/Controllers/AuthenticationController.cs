using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel registrationModel)
        {
            var result = await authenticationService.RegisterUserAsync(registrationModel);

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(Register), new { emial = registrationModel.Email }, null);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var result = await authenticationService.LoginUserAsync(loginModel);

            if (result.Succeeded)
            {
                return Ok("Login successful");
            }
            else
            {
                return Unauthorized("Invalid credentials.");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await authenticationService.LogoutUserAsync();

            return Ok(new { message = "User logged out successfully" });
        }
    }
}
