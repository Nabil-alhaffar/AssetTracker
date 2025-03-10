using AssetTracker.Models;
using Microsoft.AspNetCore.Mvc;
namespace AssetTracker.Services.Interfaces
{
    public interface IStockService
    {
        Task<TradeResult> ExecuteTradeAsync(Guid userId, TradeRequest tradeRequest);


        //Task<TradeResult> BuyStockAsync(Guid userId, string symbol, decimal quantity);
        //Task<TradeResult> SellStockAsync(Guid userId, string symbol, decimal quantity);
        //Task<TradeResult> ShortStockAsync(Guid userId, string symbol, decimal quantity);
        //Task<TradeResult>CloseShortAsync(Guid userId, string symbol, decimal quantity);

    }
}