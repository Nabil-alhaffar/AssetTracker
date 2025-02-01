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

        [HttpGet("all")]
        public async Task<ActionResult> GetAllPositionsAsync()
        {
            try
            {
                var positions = await _positionService.GetAllPositionsAsync();
                return Ok(positions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            }
        }

        [HttpGet("{symbol}")]
        public async Task<ActionResult> GetPositionBySymbolAsync(string symbol)
        {
            try
            {
                var position = await _positionService.GetPositionAsync(symbol);
                if (position == null)
                {
                    return NotFound("Position not found");
                }
                return Ok(position);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddPositionAsync([FromBody] Position position)
        {
            try
            {
                await _positionService.AddPositionAsync(position);
                return Ok("Position added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            }
        }

        [HttpDelete("remove/{symbol}")]
        public async Task<ActionResult> RemovePositionAsync(string symbol)
        {
            try
            {
                await _positionService.RemovePositionAsync(symbol);
                return Ok("Position removed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            }
        }
    }

}

