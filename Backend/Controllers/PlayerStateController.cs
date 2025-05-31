using Backend.Services;
using Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[ApiController]
[Route("api/player-state")]
public class PlayerStateController : ControllerBase
{
    private readonly IPlayerStateService _playerStateService;

    public PlayerStateController (IPlayerStateService playerStateService)
    {
        _playerStateService = playerStateService;
    }

    [Authorize]
    [HttpGet("invite/me")]
    public async Task<ActionResult<PlayerInviteTableIdResponseDTO>> GetSignedInPlayerInviteTableId()
    {
        long? id = await _playerStateService.GetSignedInPlayerInviteTableIdAsync();

        return Ok(new PlayerInviteTableIdResponseDTO { TableId = id });
    }

    [Authorize]
    [HttpGet("table/me")]
    public async Task<ActionResult<PlayerTableIdResponseDTO>> GetSignedInPlayerTableId()
    {
        long? id = await _playerStateService.GetSignedInPlayerTableIdAsync();

        return Ok(new PlayerTableIdResponseDTO { TableId = id });
    }

    [Authorize]
    [HttpGet("game/me")]
    public async Task<ActionResult<PlayerGameIdResponseDTO>> GetSignedInPlayerGameId()
    {
        long? id = await _playerStateService.GetSignedInPlayerGameIdAsync();

        return Ok(new PlayerGameIdResponseDTO { GameId = id });
    }
}
