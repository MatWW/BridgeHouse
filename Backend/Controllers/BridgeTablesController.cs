using Backend.Services;
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

    [HttpGet]
    public async Task<ActionResult<List<BridgeTable>>> GetAllBridgeTables()
    {
        List<BridgeTable> tables = await bridgeTablesService.GetAllBridgeTablesAsync();

        return Ok(tables);
    }

    [HttpGet("{bridgeTableId:long}")]
    public async Task<ActionResult<BridgeTable>> GetBridgeTable(int bridgeTableId)
    {
        BridgeTable table = await bridgeTablesService.GetBridgeTableByIdAsync(bridgeTableId);

        return Ok(table);
    }
    
    [HttpPost]
    public async Task<ActionResult<BridgeTable>> CreateBridgeTable([FromBody] int numberOfDeals)
    {
        BridgeTable createdTable = await bridgeTablesService.CreateBridgeTableAsync(numberOfDeals);

        var url = Url.Action("api/bridge-tables", new { bridgeTableId = createdTable.Id });

        return Created(url, createdTable);
    }

    [HttpPatch("{bridgeTableId:long}/add-user/{userId}")]
    public async Task<ActionResult<BridgeTable>> AddUserToBridgeTable(long bridgeTableId, string userId, Position position)
    {
        await bridgeTablesService.AddUserToBridgeTableAsync(bridgeTableId, userId, position);

        return Ok();
    }

    [HttpPatch("{bridgeTableId:long}/remove-user/{userId}")]
    public async Task<ActionResult<BridgeTable>> RemoveUserFromBridgeTable(long bridgeTableId, string userId)
    {
        await bridgeTablesService.RemoveUserFromBridgeTableAsync(bridgeTableId, userId);

        return Ok();
    }

    [HttpDelete("{bridgeTableId:long}")]
    public async Task<IActionResult> DeleteBridgeTable(long bridgeTableId)
    {
        await bridgeTablesService.DeleteBridgeTableAsync(bridgeTableId);

        return NoContent();
    }
}