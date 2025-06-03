using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Enums;
using Shared.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/bridge-tables")]
public class BridgeTableController : ControllerBase
{
    private readonly IBridgeTablesService _bridgeTablesService;

    public BridgeTableController(IBridgeTablesService bridgeTablesService)
    {
        _bridgeTablesService = bridgeTablesService;
    }

    [Authorize]
    [HttpGet("{bridgeTableId:long}")]
    public async Task<ActionResult<BridgeTable>> GetBridgeTable(int bridgeTableId)
    {
        BridgeTable table = await _bridgeTablesService.GetBridgeTableByIdAsync(bridgeTableId);

        return Ok(table);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BridgeTable>> CreateBridgeTable([FromBody] CreateBridgeTableRequestDTO request)
    {
        BridgeTable createdTable = await _bridgeTablesService.CreateBridgeTableAsync(request);

        var url = Url.Action("api/bridge-tables", new { bridgeTableId = createdTable.Id });

        return Created(url, createdTable);
    }

    [Authorize]
    [HttpPost("{bridgeTableId:long}/invite/{userId}")]
    public async Task<IActionResult> InviteUserToBridgeTable(long bridgeTableId, string userId,
        [FromBody] InvitePlayerToTableDTO dto)
    {
        Position position = dto.Position;

        await _bridgeTablesService.InviteUserToBridgeTableAsync(bridgeTableId, userId, position);

        return Ok();
    }

    [Authorize]
    [HttpPost("invite/accept/me")]
    public async Task<IActionResult> AcceptInviteToBridgeTable()
    {
        await _bridgeTablesService.AcceptInviteToBridgeTableAsync();

        return Ok();
    }

    [Authorize]
    [HttpDelete("invite/decline/me")]
    public async Task<IActionResult> DeclineInviteToBridgeTable()
    {
        await _bridgeTablesService.DeclineInviteToBridgeTableAsync();

        return Ok();
    }

    [Authorize]
    [HttpPatch("{bridgeTableId:long}/leave")]
    public async Task<IActionResult> LeaveBridgeTable(long bridgeTableId)
    {
        await _bridgeTablesService.LeaveTableAsync(bridgeTableId);

        return Ok();
    }

    [Authorize]
    [HttpPatch("{bridgeTableId:long}/remove-user/{userId}")]
    public async Task<ActionResult<BridgeTable>> RemoveUserFromBridgeTable(long bridgeTableId, string userId)
    {
        await _bridgeTablesService.RemoveUserFromBridgeTableAsync(bridgeTableId, userId);

        return Ok();
    }

    [Authorize]
    [HttpDelete("{bridgeTableId:long}")]
    public async Task<IActionResult> DeleteBridgeTable(long bridgeTableId)
    {
        await _bridgeTablesService.DeleteBridgeTableAsync(bridgeTableId);

        return NoContent();
    }
}