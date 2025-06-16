using System;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Models;
using AssetTracker.Services;
using System.Threading.Tasks;
using AssetTracker.Services.Interfaces;

namespace AssetTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionController : ControllerBase
    {

        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;

        }

        /// <summary>
        /// Splits a user's stock position based on a specified split factor.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="symbol">Stock symbol</param>
        /// <param name="splitFactor">Factor by which the position should be split</param>
        /// <returns>Success or error message</returns>
        [HttpPut("{userId}/split/{symbol}")]
        public async Task<IActionResult> SplitPositionAsync(Guid userId, string symbol, int splitFactor)
        {
            if (splitFactor <= 0)
                return BadRequest("Split factor must be a positive integer.");

            // Call the service to split the position
            await _positionService.SplitPositionAsync(userId, symbol, splitFactor);

            return Ok("Position split successfully.");
        }

        /// <summary>
        /// Checks if a stop loss condition has been triggered for a given position.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="symbol">Stock symbol</param>
        /// <param name="stopLossPrice">Stop loss threshold price</param>
        /// <returns>Boolean result indicating whether stop loss was triggered</returns>
        [HttpGet("{userId}/check-stoploss/{symbol}")]
        public async Task<IActionResult> CheckPositionForStopLossAsync(Guid userId, string symbol, [FromQuery] decimal stopLossPrice)
        {
            if (stopLossPrice <= 0)
                return BadRequest("Stop loss price must be greater than 0.");

            // Call the service to check stop loss
            bool isStopLossTriggered = await _positionService.CheckPositionForStopLossAsync(userId, symbol, stopLossPrice);

            return Ok(new { StopLossTriggered = isStopLossTriggered });
        }

        /// <summary>
        /// Updates the profit and loss for a user's specific position.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="symbol">Stock symbol</param>
        /// <returns>Success message or error</returns>
        [HttpGet("{userId}/update-profit-loss/{symbol}")]
        public async Task<IActionResult> UpdatePositionProfitLossAsync(Guid userId, string symbol)
        {
            // Call the service to update profit/loss
            await _positionService.UpdatePositionProfitLossAsync(userId, symbol);

            return Ok("Position's profit/loss updated successfully.");
        }


        /// <summary>
        /// Retrieves a summary of a user's specific stock position.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="symbol">Stock symbol</param>
        /// <returns>Position summary object or error message</returns>
        [HttpGet("{userId}/get-position-summary/{symbol}")]
        public async Task<IActionResult> GetPositionSummary(Guid userId, string symbol)
        {
            try
            {
                var position = await _positionService.GetPositionSummaryAsync(userId, symbol);
                return Ok(position);

            }
            catch (Exception e)
            {
                return BadRequest("Position Summary was not retrieved: "+ e.Message);
            }
        }

    }
}

