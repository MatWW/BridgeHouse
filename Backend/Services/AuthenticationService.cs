using Microsoft.AspNetCore.Identity;
using Shared;

namespace Backend.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<IdentityUser> userManager;

    public AuthenticationService(UserManager<IdentityUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<IdentityResult> RegisterUser(RegistrationModel registrationModel)
    {
        var user = new IdentityUser { UserName = registrationModel.UserName, Email = registrationModel.Email };
        var result = await userManager.CreateAsync(user, registrationModel.Password);

        return result;
    }
}
