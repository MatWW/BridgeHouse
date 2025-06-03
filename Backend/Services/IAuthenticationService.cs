using Microsoft.AspNetCore.Identity;
using Shared.Models;

namespace Backend.Services;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUserAsync(RegistrationModel registrationModel);
    Task<SignInResult> LoginUserAsync(LoginModel loginModel);
    Task LogoutUserAsync();
}
