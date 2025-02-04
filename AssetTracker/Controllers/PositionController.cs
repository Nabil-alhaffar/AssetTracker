using System;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Models;
using AssetTracker.Services;
using System.Threading.Tasks;

namespace AssetTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionController:ControllerBase
	{

        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
		{
            _positionService = positionService;

        }
        //public async Task<IActionResult> UpdatePositionAsync(int userId, string symbol, [FromBody] PositionUpdateRequest request)
        //{
        //    if (request == null)
        //        return BadRequest("Request data is required.");

        //    // Call the service to update the position
        //    await _positionService.UpdatePositionAsync(userId, symbol, request.AdditionalQuantity, request.PurchasePrice);

        //    return Ok("Position updated successfully.");
        //}
        [HttpPut("{userId}/split/{symbol}")]
        public async Task<IActionResult> SplitPositionAsync(int userId, string symbol, [FromBody] int splitFactor)
        {
            if (splitFactor <= 0)
                return BadRequest("Split factor must be a positive integer.");

            // Call the service to split the position
            await _positionService.SplitPositionAsync(userId, symbol, splitFactor);

            return Ok("Position split successfully.");
        }
        [HttpGet("{userId}/check-stoploss/{symbol}")]
        public async Task<IActionResult> CheckPositionForStopLossAsync(int userId, string symbol, [FromQuery] double stopLossPrice)
        {
            if (stopLossPrice <= 0)
                return BadRequest("Stop loss price must be greater than 0.");

            // Call the service to check stop loss
            bool isStopLossTriggered = await _positionService.CheckPositionForStopLossAsync(userId, symbol, stopLossPrice);

            return Ok(new { StopLossTriggered = isStopLossTriggered });
        }
        [HttpGet("{userId}/update-profit-loss/{symbol}")]
        public async Task<IActionResult> UpdatePositionProfitLossAsync(int userId, string symbol)
        {
            // Call the service to update profit/loss
            await _positionService.UpdatePositionProfitLossAsync(userId, symbol);

            return Ok("Position's profit/loss updated successfully.");
        }
        //[HttpGet("all")]
        //public async Task<ActionResult> GetAllPositionsAsync()
        //{
        //    try
        //    {
        //        var positions = await _positionService.GetAllPositionsAsync();
        //        return Ok(positions);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message); // Handle errors gracefully
        //    }
        //}

        //[HttpGet("{symbol}")]
        //public async Task<ActionResult> GetPositionBySymbolAsync(string symbol)
        //{
        //    try
        //    {
        //        var position = await _positionService.GetPositionAsync(symbol);
        //        if (position == null)
        //        {
        //            return NotFound("Position not found");
        //        }
        //        return Ok(position);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message); // Handle errors gracefully
        //    }
        //}

        //[HttpPost("add")]
        //public async Task<ActionResult> AddPositionAsync([FromBody] Position position)
        //{
        //    try
        //    {
        //        await _positionService.AddPositionAsync(position);
        //        return Ok("Position added successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message); // Handle errors gracefully
        //    }
        //}

        //    [HttpDelete("remove/{symbol}")]
        //    public async Task<ActionResult> RemovePositionAsync(string symbol)
        //    {
        //        try
        //        {
        //            await _positionService.RemovePositionAsync(symbol);
        //            return Ok("Position removed successfully");
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message); // Handle errors gracefully
        //        }
        //    }
    }

}

