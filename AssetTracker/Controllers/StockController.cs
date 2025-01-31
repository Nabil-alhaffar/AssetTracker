using System;
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
			var currentPrice = await _stockService.GetStockPriceAsync(symbol);
			if (currentPrice == 0)
			{
				return NotFound("Stock Symbol not found or API error");
			}
			return Ok(currentPrice);
		}
	}
}

