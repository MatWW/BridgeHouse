using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Enums;
using Shared.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/games")]
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
    [HttpGet("{id:long}/bidding-state")]
    public async Task<ActionResult<BiddingState>> GetBiddingState(long id)
    {
        var biddingState = await _gameService.GetBiddingStateAsync(id);

        return Ok(biddingState);
    }

    [Authorize]
    [HttpGet("{id:long}/playing-state")]
    public async Task<ActionResult<PlayingState>> GetPlayingState(long id)
    {
        var playingState = await _gameService.GetPlayingStateAsync(id);

        return Ok(playingState);
    }

    [Authorize]
    [HttpGet("{id:long}/contract")]
    public async Task<ActionResult<Contract>> GetContractAsync(long id)
    {
        var contract = await _gameService.GetContractAsync(id);

        return Ok(contract);
    }

    [Authorize]
    [HttpGet("{id:long}/cards")]
    public async Task<ActionResult<List<Card>>> GetCards(long id, [FromQuery] string player)
    {
        var playerCards = await _gameService.GetPlayerCardsAsync(id, player);

        return Ok(playerCards);
    }

    [Authorize]
    [HttpGet("{id:long}/current-player")]
    public async Task<ActionResult<Player>> GetCurrentPlayerInfo(long id)
    {
        var playerInfo = await _gameService.GetCurrentPlayerInfoAsync(id);

        return Ok(playerInfo);
    }

    [Authorize]
    [HttpGet("{id:long}/phase")]
    public async Task<ActionResult<GamePhase>> GetGamePhase(long id)
    {
        var gamePhase = await _gameService.GetGamePhaseAsync(id);

        return gamePhase;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> StartGame([FromBody] StartGameRequestDTO dto)
    {
        await _gameService.StartGameAsync(dto.tableId, dto.players);

        return Created();
    }

    [Authorize]
    [HttpPost("{id:long}/bids")]
    public async Task<IActionResult> PlaceBid(long id, [FromBody] BidAction bidAction)
    {
        await _biddingService.PlaceBidAsync(id, bidAction);

        return Ok();
    }

    [Authorize]
    [HttpPost("{id:long}/card-plays")]
    public async Task<IActionResult> PlayCard(long id, [FromBody] CardPlayAction cardPlayAction)
    {
        await _playingService.PlayCardAsync(id, cardPlayAction);

        return Ok();
    }
}
