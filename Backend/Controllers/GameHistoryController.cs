using Backend.Data.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Backend.Controllers;

[ApiController]
[Route("api/game-history")]
public class GameHistoryController : ControllerBase
{
    private readonly IGameHistoryService _gameHistoryService;

    public GameHistoryController(IGameHistoryService gameHistoryService)
    {
        _gameHistoryService = gameHistoryService;
    }

    [Authorize]
    [HttpGet("{gameId:int}")]
    public async Task<ActionResult<GameHistory>> GetGame(int gameId)
    {
        var game = await _gameHistoryService.GetGameByIdAsync(gameId);

        return Ok(game);
    }

    [Authorize]
    [HttpGet("short-info/me")]
    public async Task <ActionResult<List<PlayerGameShortInfoDTO>>> GetUserGamesShortInfo()
    {
        var shortInfos = await _gameHistoryService.GetSignedInUserGamesShortInfoAsync();

        return Ok(shortInfos);
    }
}

