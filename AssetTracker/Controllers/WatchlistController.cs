using System;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Models;
using AssetTracker.Services;
using System.Threading.Tasks;
using AssetTracker.Services.Interfaces;
using AssetTracker.Models.DTOs;

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

        /// <summary>
        /// Retrieves all watchlists for a given user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <returns>List of the user's watchlists or an error response</returns>
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

        /// <summary>
        /// Adds a new watchlist for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <param name="request">The watchlist name and optional initial symbols</param>
        /// <returns>Success or error message</returns>
        [HttpPost("{userId}")]
        public async Task<IActionResult> AddWatchlist(Guid userId, [FromBody]AddWatchlistRequest request  )
        {
            try
            {
                await _watchlistService.AddWatchlistAsync(userId, request.WatchlistName, request.Symbols);

                return Ok(new { message = "Watchlist added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while adding the watchlist.", error = ex.Message });
            }
        }



        /// <summary>
        /// Removes a specific watchlist for a user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <param name="watchlistId">The unique identifier of the watchlist</param>
        /// <returns>Success or error message</returns>
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

        /// <summary>
        /// Adds one or more symbols to a user's watchlist.
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <param name="watchlistId">The unique identifier of the watchlist</param>
        /// <param name="request">List of symbols to add</param>
        /// <returns>Success or error message</returns>
        [HttpPost("{userId}/{watchlistId}/add-symbols")]
        public async Task<IActionResult> AddSymbolToWatchlist(Guid userId, Guid watchlistId, [FromBody] AdjustWatchlistRequest request)
        {
            try
            {
                await _watchlistService.AddSymbolsToWatchlistAsync(userId, watchlistId, request.Symbols);
                return Ok(new { message = "Symbol(s) added to watchlist successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while adding the symbol to the watchlist.", error = ex.Message });
            }
        }



        /// <summary>
        /// Removes one or more symbols from a user's watchlist.
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <param name="watchlistId">The unique identifier of the watchlist</param>
        /// <param name="request">List of symbols to remove</param>
        /// <returns>Success or error message</returns>
        [HttpPost("{userId}/{watchlistId}/remove-symbols")]
        public async Task<IActionResult> RemoveSymbolFromWatchlist(Guid userId, Guid watchlistId, [FromBody] AdjustWatchlistRequest request)
        {
            try
            {
                await _watchlistService.RemoveSymbolsFromWatchlistAsync(userId, watchlistId, request.Symbols);
                return Ok(new { message = "Symbol(s) removed from watchlist successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while removing the symbol from the watchlist.", error = ex.Message });
            }
        }
    }
}
