using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserStateService _userStateService;
    private readonly IBridgeTablesService _bridgeTablesService;
    private readonly IGameService _gameService;
    private readonly IGameHistoryService _gameHistoryService;

    public UserController(IUserService userService, IUserStateService userStateService,
        IBridgeTablesService bridgeTablesService, IGameService gameService,
        IGameHistoryService gameHistoryService)
    {
        _userService = userService;
        _userStateService = userStateService;
        _bridgeTablesService = bridgeTablesService;
        _gameService = gameService;
        _gameHistoryService = gameHistoryService;
    }

    [Authorize]
    [HttpGet("me/id")]
    public ActionResult<string> GetSignedInUserId()
    {
        string id = _userService.GetCurrentUserId();

        return Ok(id);
    }

    [HttpGet("id")]
    public async Task<ActionResult<string>> GetIdByNickname([FromQuery] string nickname)
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

    [Authorize]
    [HttpGet("me/game-info")]
    public async Task<ActionResult<Player>> GetSignedInPlayerInfo()
    {
        var playerInfo = await _gameService.GetSignedInPlayerInfoAsync();

        return Ok(playerInfo);
    }

    [Authorize]
    [HttpGet("me/game-histories/short-info")]
    public async Task<ActionResult<List<PlayerGameShortInfoDTO>>> GetUserGamesShortInfo()
    {
        var shortInfos = await _gameHistoryService.GetSignedInUserGamesShortInfoAsync();

        return Ok(shortInfos);
    }
}
