using System;
using AssetTracker.Models;
using AssetTracker.Services;
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
    }
}

