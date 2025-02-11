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
        private readonly IStockService _stockService;
        public StockController(IStockService stockService)
        {
            _stockService = stockService; 
        }

        [HttpPost("{userId}/execute-trade")]
        public async Task<IActionResult> ExecuteTrade(Guid userId, TradeRequest tradeRequest)
        {



            TradeResult tradeResult = await _stockService.ExecuteTradeAsync(userId, tradeRequest);
            return tradeResult.Success ? Ok(tradeResult) : BadRequest(tradeResult);

        }

        //[HttpPost("{userId}/buy")]
        //public async Task<IActionResult> BuyStock(Guid userId, [FromBody] TradeRequest request)
        //{
        //    TradeResult tradeResult = await _stockService.BuyStockAsync(userId, request.Symbol, request.Quantity);

        //    return tradeResult.Success ? Ok(tradeResult) : BadRequest(tradeResult);
        //}

        //[HttpPost("{userId}/sell")]
        //public async Task<IActionResult> SellStock(Guid userId, [FromBody] TradeRequest request)
        //{
        //    TradeResult tradeResult = await _stockService.SellStockAsync(userId, request.Symbol, request.Quantity);

        //    return tradeResult.Success ? Ok(tradeResult) : BadRequest(tradeResult);
        //}
        //[HttpPost("{userId}/short")]
        //public async Task<IActionResult> ShortStock(Guid userId, [FromBody] TradeRequest request)
        //{
        //    TradeResult tradeResult = await _stockService.ShortStockAsync(userId, request.Symbol, request.Quantity);

        //    return tradeResult.Success ? Ok(tradeResult) : BadRequest(tradeResult);
        //}
        //[HttpPost("{userId}/close-short")]
        //public async Task<IActionResult> CloseShort(Guid userId, [FromBody] TradeRequest request)
        //{
        //    TradeResult tradeResult = await _stockService.CloseShortAsync(userId, request.Symbol, request.Quantity);

        //    return tradeResult.Success ? Ok(tradeResult) : BadRequest(tradeResult);
        //}
    }
}

