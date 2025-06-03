using Microsoft.AspNetCore.Identity;

namespace Backend.Data.Models;

public class AppUser : IdentityUser
{
    public string Nickname { get; set; } = string.Empty;
}
