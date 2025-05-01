using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

public class AppUser : IdentityUser
{
    public string Nickname { get; set; } = string.Empty;
}
