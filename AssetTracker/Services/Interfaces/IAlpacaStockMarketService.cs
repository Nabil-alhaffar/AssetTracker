using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace AssetTracker.Services.Interfaces
{
    public interface IAlpacaStockMarketService
    {
        public Task SubscribeToStockAsync(string symbol, CancellationToken stoppingToken);

    }
}