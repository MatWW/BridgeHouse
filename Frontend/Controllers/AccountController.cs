using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    [HttpGet]
    public IActionResult Welcome()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)  
        {
            return Ok("You are not authenticated");
        }

        return Ok("You are authenticated");
    }
}
