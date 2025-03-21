using System;
using System.Net.Http;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AssetTracker.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController: ControllerBase
	{
        private readonly IStockService _stockService;
        public StockController(IStockService stockService)
        {
            _stockService = stockService; 
        }

        [HttpPost("execute-trade")]
        [Authorize]
        public async Task<IActionResult> ExecuteTrade( TradeRequest tradeRequest)
        {


            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Get the userId from the JWT token

            TradeResult tradeResult = await _stockService.ExecuteTradeAsync(userId, tradeRequest);
            return tradeResult.Success ? Ok(tradeResult) : BadRequest(tradeResult);

        }

        
    }
}

