using System;
using System.Net.Http;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracker.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController: ControllerBase
	{

        private static readonly string[] DefaultIndicators = { "SMA", "EMA", "MACD", "RSI", "BBANDS" };

        public readonly IStockService _stockService;
		public StockController(IStockService stockService)

		{
			_stockService = stockService; 
		}

		[HttpGet("getPrice/{symbol}")]
		public async Task<IActionResult> GetStockPrice(string symbol)
		{
			var currentPrice = await _stockService.GetStockPriceAsync(symbol);
			if (currentPrice == 0)
			{
				return NotFound("Stock Symbol not found or API error");
			}
			return Ok(currentPrice);
		}

        [HttpGet("getStock/{symbol}")]
        public async Task<IActionResult> GetStockDetails(string symbol)
        {
            var stock = await _stockService.GetStockOverviewAsync(symbol); // synchronous
            if (stock == null)
            {
                return NotFound("Stock not found.");
            }

            // Get the logo URL
            stock.LogoURL = _stockService.GetCompanyLogoUrl(stock.OfficialSite);

            return Ok(new { stock, logoUrl = stock.LogoURL });
        }

        [HttpGet("stock/{symbol}/historical/{interval}")]
        public async Task<ActionResult<IEnumerable<HistoricalData>>> GetHistoricalData(string symbol, string interval)
        {
            var historicalData = await _stockService.GetHistoricalDataAsync(symbol, interval);
            if (historicalData == null || !historicalData.Any())
            {
                return NotFound("Historical data not found.");
            }

            return Ok(historicalData);
        }

        [HttpGet("indicators")]
        public async Task<IActionResult> GetStockIndicators([FromQuery] string symbol,[FromQuery] string interval = "daily", [FromQuery] int timePeriod = 14, [FromQuery] string[] indicators = null, [FromQuery]int limit =100)
        {
            if (string.IsNullOrEmpty(symbol))
                return BadRequest(new { message = "Symbol is required." });

            indicators ??= DefaultIndicators; // Use default indicators if none are provided

            var data = await _stockService.GetStockIndicatorsAsync(symbol, indicators.ToList(), interval, timePeriod, limit);

            if (!data.Any())
                return NotFound(new { message = "No data found." });

            return Ok(new { symbol, interval, timePeriod, limit, indicators, data });
        }

    }
}

