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
		public readonly IStockService _stockService;
		public StockController(IStockService stockService)

		{
			_stockService = stockService; 
		}

		[HttpGet("getPrice/{symbol}")]
		public async Task<IActionResult> GetStockPrice(string symbol)
		{
			var currentPrice = await _stockService.GetStockOverviewAsync(symbol);
			if (currentPrice.CurrentPrice == 0)
			{
				return NotFound("Stock Symbol not found or API error");
			}
			return Ok(currentPrice.CurrentPrice);
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
            var logoUrl = _stockService.GetCompanyLogoUrl(stock.OfficialSite);
            stock.LogoURL = logoUrl;
            return Ok(stock);
            //return new
            //{
            //    Stock = stock,
            //    LogoUrl = logoUrl
            //});
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
        public async Task<IActionResult> GetStockIndicators([FromQuery] string symbol,[FromQuery] string interval = "daily", [FromQuery] int timePeriod = 14, [FromQuery] string[] indicators = null)
        {
            if (string.IsNullOrEmpty(symbol))
                return BadRequest("Symbol is required.");

            if (indicators == null || indicators.Length == 0)
                indicators = new string[] { "SMA", "EMA", "MACD", "RSI", "BBANDS" };

           ;
            var data = await _stockService.GetStockIndicatorsAsync(symbol, indicators.ToList(), interval, timePeriod);

            if (!data.Any())
                return NotFound("Error Fetching Data.");

            return Ok(data);
        }

    }
}

