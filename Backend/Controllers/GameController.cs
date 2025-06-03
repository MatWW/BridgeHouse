using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Enums;
using Shared.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/game")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IBiddingService _biddingService;
    private readonly IPlayingService _playingService;

    public GameController(IGameService gameService, IBiddingService biddingService, IPlayingService playingService)
    {
        _gameService = gameService;
        _biddingService = biddingService;
        _playingService = playingService;
    }

    [Authorize]
    [HttpGet("{gameId:long}/biddingState")]
    public async Task<ActionResult<BiddingState>> GetBiddingState(long gameId)
    {
        var biddingState = await _gameService.GetBiddingStateAsync(gameId);

        return Ok(biddingState);
    }

    [Authorize]
    [HttpGet("{gameId:long}/playingState")]
    public async Task<ActionResult<PlayingState>> GetPlayingState(long gameId)
    {
        var playingState = await _gameService.GetPlayingStateAsync(gameId);

        return Ok(playingState);
    }

    [Authorize]
    [HttpGet("{gameId:long}/contract")]
    public async Task<ActionResult<Contract>> GetContractAsync(long gameId)
    {
        var contract = await _gameService.GetContractAsync(gameId);

        return Ok(contract);
    }

    [Authorize]
    [HttpGet("{gameId:long}/cards/me")]
    public async Task<ActionResult<List<Card>>> GetPlayerCards(long gameId)
    {
        var playerCards = await _gameService.GetSignedInPlayerCardsAsync(gameId);

        return Ok(playerCards);
    }

    [Authorize]
    [HttpGet("{gameId:long}/cards/dummy")]
    public async Task<ActionResult<List<Card>>> GetDummiesCards(long gameId)
    {
        var dummiesCards = await _gameService.GetDummiesCardsAsync(gameId);

        return Ok(dummiesCards);
    }

    [Authorize]
    [HttpGet("{gameId:long}/playerInfo/current")]
    public async Task<ActionResult<Player>> GetCurrentPlayerInfo(long gameId)
    {
        var playerInfo = await _gameService.GetCurrentPlayerInfoAsync(gameId);

        return Ok(playerInfo);
    }

    [Authorize]
    [HttpGet("{gameId:long}/playerInfo/me")]
    public async Task<ActionResult<Player>> GetSignedInPlayerInfo(long gameId)
    {
        var playerInfo = await _gameService.GetSignedInPlayerInfoAsync(gameId);

        return Ok(playerInfo);
    }

    [Authorize]
    [HttpGet("{gameId:long}/phase")]
    public async Task<ActionResult<GamePhase>> GetGamePhase(long gameId)
    {
        var gamePhase = await _gameService.GetGamePhaseAsync(gameId);

        return gamePhase;
    }

    [Authorize]
    [HttpPost("startGame")]
    public async Task<IActionResult> StartGame([FromBody] StartGameRequestDTO dto)
    {
        await _gameService.StartGameAsync(dto.tableId, dto.players);

        return Created();
    }

    [Authorize]
    [HttpPost("{gameId:long}/bid")]
    public async Task<IActionResult> PlaceBid(long gameId, [FromBody] BidAction bidAction)
    {
        await _biddingService.PlaceBidAsync(gameId, bidAction);

        return Ok();
    }

    [Authorize]
    [HttpPost("{gameId:long}/playCard")]
    public async Task<IActionResult> PlayCard(long gameId, [FromBody] CardPlayAction cardPlayAction)
    {
        await _playingService.PlayCardAsync(gameId, cardPlayAction);

        return Ok();
    }
}
