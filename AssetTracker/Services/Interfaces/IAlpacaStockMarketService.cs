using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Services.Interfaces
{
    public interface IAlpacaStockMarketService
    {
        //public Task SubscribeToStockAsync(string symbol, CancellationToken stoppingToken);
        public Task<string> GetSnapshotAsync(string symbol);
        public Task<List<AlpacaNewsItem>> GetNewsAsync(string symbol, int limit = 20);

        public Task<string> GetHistoricalBarsAsync(string symbol, string timeframe = "1Day", string start = "2024-01-01");
        public Task<AlpacaMostActiveResponse> GetMostActivesAsync();
        public Task<AlpacaMarketMoversResponse> GetMarketMoversAsync(string marketType);

        public Task<List<SymbolLookupResult>> SearchAsync(string query);
        public Task InitializeAsync();

    }
}