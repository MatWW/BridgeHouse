using Microsoft.AspNetCore.Identity;
using Shared;

namespace Backend.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;

    public AuthenticationService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    public async Task<IdentityResult> RegisterUserAsync(RegistrationModel registrationModel)
    {
        var user = new IdentityUser { UserName = registrationModel.UserName, Email = registrationModel.Email };
        var result = await userManager.CreateAsync(user, registrationModel.Password);

        return result;
    }

    public async Task<SignInResult> LoginUserAsync(LoginModel loginModel)
    {
        var result = await signInManager.PasswordSignInAsync(loginModel.UserName, loginModel.Password, isPersistent: false, lockoutOnFailure: false);

        return result;
    }

    public async Task LogoutUserAsync()
    {
        await signInManager.SignOutAsync();
    }
}
