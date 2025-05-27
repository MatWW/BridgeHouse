using Microsoft.AspNetCore.Identity;
using Shared;
using Backend.Models;

namespace Backend.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<AppUser> userManager;
    private readonly SignInManager<AppUser> signInManager;

    public AuthenticationService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    public async Task<IdentityResult> RegisterUserAsync(RegistrationModel registrationModel)
    {
        var user = new AppUser
        {
            Email = registrationModel.Email,
            UserName = registrationModel.Email,
            Nickname = registrationModel.Nickname
        };

        var result = await userManager.CreateAsync(user, registrationModel.Password);

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, isPersistent: false);
        }

        return result;
    }

    public async Task<SignInResult> LoginUserAsync(LoginModel loginModel)
    {
        var result = await signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, isPersistent: true, lockoutOnFailure: false);

        return result;
    }

    public async Task LogoutUserAsync()
    {
        await signInManager.SignOutAsync();
    }
}
