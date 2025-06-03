using System;
using AssetTracker.Services.Interfaces;
using AssetTracker.Services;
using AssetTracker.Helpers;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Models;

namespace AssetTracker.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SymbolSubscriptionController:ControllerBase
	{
		private readonly SymbolSubscriptionManager _symbolSubscriptionManager;

		public SymbolSubscriptionController(SymbolSubscriptionManager symbolSubscriptionManager)
		{
			_symbolSubscriptionManager = symbolSubscriptionManager;
		}

		[HttpPost("{userId}/subscribe-to-Symbol/{symbol}")]
        public async Task <IActionResult>  SubscribeUserToSymbol(Guid userId, string symbol)
		{
			try
			{
				await _symbolSubscriptionManager.SubscribeUserToSymbolAsync(userId, symbol);
				return Ok($"{userId} is now subscribed to {symbol}");
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
        [HttpPost("{userId}/Unsubscribe-from-symbol/{symbol}")]
        public async Task <IActionResult> UnsubscribeUserFromSymbol(Guid userId, string symbol)
		{
            try
            {
                await _symbolSubscriptionManager.UnsubscribeUserFromSymbolAsync(userId, symbol);
                return Ok($"{userId} is now unsubscribed from {symbol}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        [HttpPost("{userId}/Unsubscribe-from-all")]
        public async Task <IActionResult> UnsubscribeUserFromAll(Guid userId)
		{
            try
            {
                await _symbolSubscriptionManager.UnsubscribeUserFromAllAsync(userId);
                return Ok($"{userId} is now unsubscribed from all tickers.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        [HttpGet("/{userId}/symbols")]
        public async Task<IActionResult> GetUserSubscribedSymbols(Guid userId)
        {
            try
            {
                var result =  _symbolSubscriptionManager.GetUserSubscribedSymbols(userId);
                
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("/{symbol}/subscribed-users")]
        public async Task<IActionResult> GetUsersSubscribedToSymbol(string symbol)
        {
            try
            {
                var result = _symbolSubscriptionManager.GetUsersSubscribedToSymbol(symbol);

                return Ok(new { result });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}

