using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Backend.Controllers;

[ApiController]
[Route("api/game")]
public class GameController : ControllerBase
{
    private readonly IGameService gameService;
    private readonly IBiddingService biddingService;
    private readonly IPlayingService playingService;

    public GameController(IGameService gameService, IBiddingService biddingService, IPlayingService playingService)
    {
        this.gameService = gameService;
        this.biddingService = biddingService;
        this.playingService = playingService;
    }

    [HttpGet("{gameId:long}/gameState")]
    public async Task<ActionResult<GameState>> GetGameState(long gameId)
    {
        var gameState = await gameService.GetGameStateAsync(gameId);

        return Ok(gameState);
    }

    [HttpGet("{gameId:long}/biddingState")]
    public async Task<ActionResult<BiddingState>> GetBiddingState(long gameId)
    {
        var biddingState = await gameService.GetBiddingStateAsync(gameId);

        return Ok(biddingState);
    }

    [HttpGet("{gameId:long}/playingState")]
    public async Task<ActionResult<PlayingState>> GetPlayingState(long gameId)
    {
        var playingState = await gameService.GetPlayingStateAsync(gameId);

        return Ok(playingState);
    }

    [HttpGet("{gameId:long}/contract")]
    public async Task<ActionResult<PlayingState>> GetContractAsync(long gameId)
    {
        var contract = await gameService.GetContractAsync(gameId);

        return Ok(contract);
    }

    [HttpGet("{gameId:long}/cards/{playerId}")]
    public async Task<ActionResult<List<Card>>> GetPlayerCards(long gameId, string playerId)
    {
        var playerCards = await gameService.GetPlayerCardsAsync(gameId, playerId);

        return Ok(playerCards);
    }

    [HttpGet("{gameId:long}/cards/dummy")]
    public async Task<ActionResult<List<Card>>> GetDummiesCards(long gameId)
    {
        var dummiesCards = await gameService.GetDummiesCardsAsync(gameId);

        return Ok(dummiesCards);
    }

    [HttpGet("{gameId:long}/playerInfo/{playerId}")]
    public async Task<ActionResult<Player>> GetPlayerInfo(long gameId, string playerId)
    {
        var playerInfo = await gameService.GetPlayerInfoAsync(gameId, playerId);

        return Ok(playerInfo);
    }

    [HttpGet("{gameId:long}/playerInfo/current")]
    public async Task<ActionResult<Player>> GetCurrentPlayerInfo(long gameId)
    {
        var playerInfo = await gameService.GetCurrentPlayerInfoAsync(gameId);

        return Ok(playerInfo);
    }

    [HttpGet("{gameId:long}/playerInfo/me")]
    public async Task<ActionResult<Player>> GetSignedInPlayerInfo(long gameId)
    {
        var playerInfo = await gameService.GetSignedInPlayerInfoAsync(gameId);

        return Ok(playerInfo);
    }

    [HttpPost("startGame")]
    public async Task<IActionResult> StartGame([FromBody] StartGameRequestDTO dto)
    {
        await gameService.StartGameAsync(dto.tableId, dto.players);

        return Created();
    }

    [HttpPost("{gameId:long}/bid")]
    public async Task<IActionResult> PlaceBid(long gameId, [FromBody] BidAction bidAction)
    {
        await biddingService.PlaceBidAsync(gameId, bidAction);

        return Ok();
    }

    [HttpPost("{gameId:long}/playCard")]
    public async Task<IActionResult> PlayCard(long gameId, [FromBody] CardPlayAction cardPlayAction)
    {
        await playingService.PlayCardAsync(gameId, cardPlayAction);

        return Ok();
    }

    [HttpGet("me")]
    public async Task<ActionResult<long?>> GetSignedInPlayerGameInfo()
    {
        long? gameId = await gameService.GetSignedInPlayerGameIdAsync();

        return Ok(gameId);
    }
}
