using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Enums;

namespace Backend.Controllers;

[ApiController]
[Route("api/bridge-tables")]
public class BridgeTableController : ControllerBase
{
    private readonly IBridgeTablesService bridgeTablesService;

    public BridgeTableController(IBridgeTablesService bridgeTablesService)
    {
        this.bridgeTablesService = bridgeTablesService;
    }

    [Authorize]
    [HttpGet("{bridgeTableId:long}")]
    public async Task<ActionResult<BridgeTable>> GetBridgeTable(int bridgeTableId)
    {
        BridgeTable table = await bridgeTablesService.GetBridgeTableByIdAsync(bridgeTableId);

        return Ok(table);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BridgeTable>> CreateBridgeTable([FromBody] CreateBridgeTableRequestDTO request)
    {
        BridgeTable createdTable = await bridgeTablesService.CreateBridgeTableAsync(request);

        var url = Url.Action("api/bridge-tables", new { bridgeTableId = createdTable.Id });

        return Created(url, createdTable);
    }

    [Authorize]
    [HttpPost("{bridgeTableId:long}/invite/{userId}")]
    public async Task<IActionResult> InviteUserToBridgeTable(long bridgeTableId, string userId,
        [FromBody] InvitePlayerToTableDTO dto)
    {
        Position position = dto.Position;

        await bridgeTablesService.InviteUserToBridgeTableAsync(bridgeTableId, userId, position);

        return Ok();
    }

    [Authorize]
    [HttpPost("{bridgeTableId:long}/invite/{userId}/accept")]
    public async Task<IActionResult> AcceptInviteToBridgeTable(long bridgeTableId, string userId)
    {
        await bridgeTablesService.AcceptInviteToBridgeTableAsync(bridgeTableId, userId);

        return Ok();
    }

    [Authorize]
    [HttpDelete("invite/{userId}/decline")]
    public async Task<IActionResult> DeclineInviteToBridgeTable(string userId)
    {
        await bridgeTablesService.DeclineInviteToBridgeTableAsync(userId);

        return Ok();
    }

    [Authorize]
    [HttpPatch("{bridgeTableId:long}/remove-user/{userId}")]
    public async Task<ActionResult<BridgeTable>> RemoveUserFromBridgeTable(long bridgeTableId, string userId)
    {
        await bridgeTablesService.RemoveUserFromBridgeTableAsync(bridgeTableId, userId);

        return Ok();
    }

    [Authorize]
    [HttpDelete("{bridgeTableId:long}")]
    public async Task<IActionResult> DeleteBridgeTable(long bridgeTableId)
    {
        await bridgeTablesService.DeleteBridgeTableAsync(bridgeTableId);

        return NoContent();
    }
}