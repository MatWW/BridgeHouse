using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel registrationModel)
        {
            var result = await authenticationService.RegisterUser(registrationModel);

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(Register), new { userName = registrationModel.UserName }, null);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var result = await authenticationService.LoginUser(loginModel);

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
            await authenticationService.LogoutUser();

            return Ok(new { message = "User logged out successfully" });
        }
    }
}
