using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpGet("id/me")]
    public ActionResult<string> GetSignedInUserId()
    {
        string id = _userService.GetCurrentUserId();

        return Ok(id);
    }

    [HttpGet("id/{nickname}")]
    public async Task<ActionResult<string>> GetIdByNickname(string nickname)
    {
        string id = await _userService.GetUserIdByNicknameAsync(nickname);

        return Ok(id);
    }

}
