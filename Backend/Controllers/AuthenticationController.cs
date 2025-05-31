using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel registrationModel)
        {
            var result = await _authenticationService.RegisterUserAsync(registrationModel);

            return result.Succeeded ? CreatedAtAction(nameof(Register), new { email = registrationModel.Email }, null) 
                : BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var result = await _authenticationService.LoginUserAsync(loginModel);

            return result.Succeeded ? Ok("Login Successful") : Unauthorized("Invalid credentials");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authenticationService.LogoutUserAsync();

            return Ok("User logged out successfully");
        }
    }
}
