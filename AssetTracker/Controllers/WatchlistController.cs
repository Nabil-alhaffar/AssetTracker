using System;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Models;
using AssetTracker.Services;
using System.Threading.Tasks;
using AssetTracker.Services.Interfaces;

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
        public async Task<ActionResult<List<Watchlist>>> GetUserWatchlists(Guid userId)
        {
            try
            {
                var watchlists = await _watchlistService.GetUserWatchlistsAsync(userId);
                //if (watchlists == null || watchlists.Count == 0)
                //{
                //    return NotFound(new { message = "No watchlists found for the given user." });
                //}

                return Ok(watchlists);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the watchlists.", error = ex.Message });
            }
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> AddWatchlist(Guid userId, [FromBody] Watchlist watchlist)
        {
            try
            {
                await _watchlistService.AddWatchlistAsync(userId, watchlist);
                return Ok(new { message = "Watchlist added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while adding the watchlist.", error = ex.Message });
            }
        }

        [HttpDelete("{userId}/{watchlistId}")]
        public async Task<IActionResult> RemoveWatchlist(Guid userId, Guid watchlistId)
        {
            try
            {
                await _watchlistService.RemoveWatchlistAsync(userId, watchlistId);
                return Ok(new { message = "Watchlist removed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while removing the watchlist.", error = ex.Message });
            }
        }

        [HttpPost("{userId}/{watchlistId}/add-symbol")]
        public async Task<IActionResult> AddSymbolToWatchlist(Guid userId, Guid watchlistId, [FromBody] string symbol)
        {
            try
            {
                await _watchlistService.AddSymbolToWatchlistAsync(userId, watchlistId, symbol);
                return Ok(new { message = "Symbol added to watchlist successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while adding the symbol to the watchlist.", error = ex.Message });
            }
        }

        [HttpPost("{userId}/{watchlistId}/remove-symbol")]
        public async Task<IActionResult> RemoveSymbolFromWatchlist(Guid userId, Guid watchlistId, [FromBody] string symbol)
        {
            try
            {
                await _watchlistService.RemoveSymbolFromWatchlistAsync(userId, watchlistId, symbol);
                return Ok(new { message = "Symbol removed from watchlist successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while removing the symbol from the watchlist.", error = ex.Message });
            }
        }
    }
}
