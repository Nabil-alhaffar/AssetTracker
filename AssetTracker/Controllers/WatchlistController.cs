using System;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Models;
using AssetTracker.Services;
using System.Threading.Tasks;

namespace AssetTracker.Controllers
{
    [ApiController]
    [Route("api/watchlists")]
    public class WatchlistController : ControllerBase
    {
        private readonly IWatchlistService _watchlistService;

        public WatchlistController(IWatchlistService watchlistService)
        {
            _watchlistService = watchlistService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<Watchlist>>> GetUserWatchlists(string userId)
        {
            return await _watchlistService.GetUserWatchlistsAsync(userId);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> AddWatchlist(string userId, [FromBody] Watchlist watchlist)
        {
            await _watchlistService.AddWatchlistAsync(userId, watchlist);
            return Ok();
        }

        [HttpDelete("{userId}/{watchlistId}")]
        public async Task<IActionResult> RemoveWatchlist(string userId, string watchlistId)
        {
            await _watchlistService.RemoveWatchlistAsync(userId, watchlistId);
            return Ok();
        }

        [HttpPost("{userId}/{watchlistId}/add-symbol")]
        public async Task<IActionResult> AddSymbolToWatchlist(string userId, string watchlistId, [FromBody] string symbol)
        {
            await _watchlistService.AddSymbolToWatchlistAsync(userId, watchlistId, symbol);
            return Ok();
        }

        [HttpPost("{userId}/{watchlistId}/remove-symbol")]
        public async Task<IActionResult> RemoveSymbolFromWatchlist(string userId, string watchlistId, [FromBody] string symbol)
        {
            await _watchlistService.RemoveSymbolFromWatchlistAsync(userId, watchlistId, symbol);
            return Ok();
        }
    }

}

