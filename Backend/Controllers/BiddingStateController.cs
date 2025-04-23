using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/bidding-state")]
    public class BiddingStateController : Controller
    {
        private readonly IBiddingStateService biddingStateService;

        public BiddingStateController(IBiddingStateService biddingStateService)
        {
            this.biddingStateService = biddingStateService;
        }

        [HttpGet("current")]
        public async Task<ActionResult<BiddingState?>> GetCurrentBiddingState()
        {
            var state = await biddingStateService.GetCurrentBiddingStateAsync();

            return Ok(state);
        }

        [HttpPost("current")]
        public async Task<IActionResult> SetCurrentBiddingState([FromBody] BiddingState newState)
        {
            await biddingStateService.SetCurrentBiddingStateAsync(newState);
            return NoContent();
        }
    }
}
