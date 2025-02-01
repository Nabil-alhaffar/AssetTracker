using System;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;

        // Constructor with Dependency Injection
        public PortfolioController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        // Get the total portfolio value
        [HttpGet("total-value")]
        public async Task<ActionResult<double>> GetTotalValue()
        {
            try
            {
                var totalValue = await _portfolioService.GetTotalValueAsync();
                return Ok(totalValue);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            }
        }

        // Get the total Profit & Loss
        [HttpGet("total-pnl")]
        public async Task<ActionResult<double>> GetTotalProfitAndLoss()
        {
            try
            {
                var totalProfitLoss = await _portfolioService.GetTotalProfitAndLossAsync();
                return Ok(totalProfitLoss);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            }
        }

        // Add a new position to the portfolio
        [HttpPost("add-position")]
        public async Task<ActionResult> AddPositionToPortfolio([FromBody] Position position)
        {
            try
            {
                await _portfolioService.AddPositionToPortfolioAsync(position);
                return Ok("Position added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            }
        }

        [HttpDelete("remove-position/{symbol}")]
        public async Task<ActionResult> RemovePositionAsync(string symbol)
        {
            try
            {
                await _portfolioService.RemovePositionAsync(symbol);
                return Ok("Position removed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            }
        }
        [HttpGet("all")]
        public async Task<ActionResult> GetAllPositionsAsync()
        {
            try
            {
                var positions = await _portfolioService.GetAllPositionsAsync();
                return Ok(positions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            }
        }
    }
}

