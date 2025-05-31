using Microsoft.AspNetCore.Identity;
using Shared;
using Backend.Models;

namespace Backend.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AuthenticationService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IdentityResult> RegisterUserAsync(RegistrationModel registrationModel)
    {
        var user = new AppUser
        {
            Email = registrationModel.Email,
            UserName = registrationModel.Email,
            Nickname = registrationModel.Nickname
        };

        var result = await _userManager.CreateAsync(user, registrationModel.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: true);
        }

        return result;
    }

    public async Task<SignInResult> LoginUserAsync(LoginModel loginModel)
    {
        var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, isPersistent: true, lockoutOnFailure: false);

        return result;
    }

    public async Task LogoutUserAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
