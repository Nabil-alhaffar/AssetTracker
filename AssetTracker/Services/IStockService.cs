﻿using AssetTracker.Models;
using Microsoft.AspNetCore.Mvc;

public interface IStockService
{

    Task<TradeResult> BuyStockAsync(Guid userId, string symbol, decimal quantity);
    Task<TradeResult> SellStockAsync(Guid userId, string symbol, decimal quantity);
    Task<TradeResult> ShortStockAsync(Guid userId, string symbol, decimal quantity);
    Task<TradeResult>CloseShortAsync(Guid userId, string symbol, decimal quantity);

}