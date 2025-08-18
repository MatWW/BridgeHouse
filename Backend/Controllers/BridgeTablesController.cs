using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.DTOs;
using Backend.Models;

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
    [HttpGet("{id:long}")]
    public async Task<ActionResult<BridgeTable>> GetBridgeTable(int id)
    {
        BridgeTable table = await _bridgeTablesService.GetBridgeTableByIdAsync(id);

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
    [HttpPost("{id:long}/invitations")]
    public async Task<IActionResult> InviteUserToBridgeTable(long id, [FromBody] CreateInvitationDTO dto)
    {
        await _bridgeTablesService.InviteUserToBridgeTableAsync(id, dto.UserId, dto.Position!.Value);

        return Ok();
    }

    [Authorize]
    [HttpDelete("{id:long}/users/me")]
    public async Task<IActionResult> LeaveBridgeTable(long id)
    {
        await _bridgeTablesService.LeaveTableAsync(id);

        return Ok();
    }

    [Authorize]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteBridgeTable(long id)
    {
        await _bridgeTablesService.DeleteBridgeTableAsync(id);

        return NoContent();
    }
}