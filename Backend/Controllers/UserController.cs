using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace Backend.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserStateService _userStateService;
    private readonly IBridgeTablesService _bridgeTablesService;

    public UserController(IUserService userService, IUserStateService userStateService, IBridgeTablesService bridgeTablesService)
    {
        _userService = userService;
        _userStateService = userStateService;
        _bridgeTablesService = bridgeTablesService;
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

    [HttpGet("me/state")]
    public async Task<ActionResult<UserStateDTO>> GetUserState()
    {
        UserStateDTO userState = await _userStateService.GetUserStateAsync();
        
        return Ok(userState);
    }

    [Authorize]
    [HttpPatch("me/invitation")]
    public async Task<IActionResult> AcceptInvitationToBridgeTable([FromBody] AcceptInvitationDTO dto)
    {
        await _bridgeTablesService.AcceptInviteToBridgeTableAsync(dto.InvitationSataus);

        return Ok();
    }

    [Authorize]
    [HttpDelete("me/invitation")]
    public async Task<IActionResult> DeclineInvitationToBridgeTable()
    {
        await _bridgeTablesService.DeclineInviteToBridgeTableAsync();

        return Ok();
    }
}
