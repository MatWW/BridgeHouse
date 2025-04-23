using Microsoft.AspNetCore.Identity;
using Shared;

namespace Backend.Services;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUser(RegistrationModel registrationModel);
}
