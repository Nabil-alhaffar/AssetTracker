using System;
using System.Net.Http;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services.Interfaces;

namespace AssetTracker.Controllers
{

    /// <summary>
    /// Controller to interact with AlphaVantage stock market API for stock prices, details, historical data, and indicators.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AlphaVantageStockMarketController:ControllerBase
	{
        private static readonly string[] DefaultIndicators = { "SMA", "EMA", "MACD", "RSI", "BBANDS" };
        public readonly IAlphaVantageStockMarketService _alphaVantageStockMarketService;


        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaVantageStockMarketController"/> class.
        /// </summary>
        /// <param name="alphaVantageStockMarketService">The stock market service for AlphaVantage API.</param>
        public AlphaVantageStockMarketController(IAlphaVantageStockMarketService alphaVantageStockMarketService)
		{
            _alphaVantageStockMarketService = alphaVantageStockMarketService;
		}


        /// <summary>
        /// Gets the current stock price for the specified symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The current price of the stock.</returns>
        /// <response code="200">Returns the current stock price.</response>
        /// <response code="404">If the stock symbol is not found or API error occurs.</response>
        [HttpGet("getPrice/{symbol}")]
        public async Task<IActionResult> GetStockPrice(string symbol)
        {
            var currentPrice = await _alphaVantageStockMarketService.GetStockPriceAsync(symbol);
            if (currentPrice == 0)
            {
                return NotFound("Stock Symbol not found or API error");
            }
            return Ok(currentPrice);
        }


        /// <summary>
        /// Gets detailed stock information including company overview and logo.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>Stock details and logo URL.</returns>
        /// <response code="200">Returns the stock details.</response>
        /// <response code="404">If the stock is not found.</response>
        [HttpGet("getStock/{symbol}")]
        public async Task<IActionResult> GetStockDetails(string symbol)
        {
            var stock = await _alphaVantageStockMarketService.GetStockOverviewAsync(symbol); // synchronous
            if (stock == null)
            {
                return NotFound("Stock not found.");
            }

            // Get the logo URL
            stock.LogoURL = _alphaVantageStockMarketService.GetCompanyLogoUrl(stock.OfficialSite);

            return Ok(new { stock, logoUrl = stock.LogoURL });
        }


        /// <summary>
        /// Gets historical stock data for the specified symbol and interval.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <param name="interval">The interval for historical data (e.g., daily, weekly).</param>
        /// <returns>List of historical data points.</returns>
        /// <response code="200">Returns historical stock data.</response>
        /// <response code="404">If no historical data is found.</response>
        [HttpGet("stock/{symbol}/historical/{interval}")]
        public async Task<ActionResult<IEnumerable<HistoricalData>>> GetHistoricalData(string symbol, string interval)
        {
            var historicalData = await _alphaVantageStockMarketService.GetHistoricalDataAsync(symbol, interval);
            if (historicalData == null || !historicalData.Any())
            {
                return NotFound("Historical data not found.");
            }

            return Ok(historicalData);
        }


        /// <summary>
        /// Gets technical indicators data for the specified stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <param name="interval">The interval for the indicators (default is "daily").</param>
        /// <param name="timePeriod">The time period for indicators (default is 14).</param>
        /// <param name="indicators">List of indicators to retrieve (default is SMA, EMA, MACD, RSI, BBANDS).</param>
        /// <param name="limit">Limit on number of data points to return (default is 100).</param>
        /// <returns>Technical indicators data.</returns>
        /// <response code="200">Returns technical indicators data.</response>
        /// <response code="400">If the symbol parameter is missing.</response>
        /// <response code="404">If no indicator data is found.</response>
        [HttpGet("indicators")]
        public async Task<IActionResult> GetStockIndicators([FromQuery] string symbol, [FromQuery] string interval = "daily", [FromQuery] int timePeriod = 14, [FromQuery] string[] indicators = null, [FromQuery] int limit = 100)
        {
            if (string.IsNullOrEmpty(symbol))
                return BadRequest(new { message = "Symbol is required." });

            indicators ??= DefaultIndicators; // Use default indicators if none are provided

            var data = await _alphaVantageStockMarketService.GetStockIndicatorsAsync(symbol, indicators.ToList(), interval, timePeriod, limit);

            if (!data.Any())
                return NotFound(new { message = "No data found." });

            return Ok(new { symbol, interval, timePeriod, limit, indicators, data });
        }
    }
}










