using System;
using AssetTracker.Models;

namespace AssetTracker.Services
{
	public interface IAlphaVantageStockMarketService
	{
        Task<decimal> GetStockPriceAsync(string symbol);
        Task<Stock> GetStockOverviewAsync(string symbol);
        string GetCompanyLogoUrl(string website);
        Task<IEnumerable<HistoricalData>> GetHistoricalDataAsync(string symbol, string interval);
        Task<Dictionary<string, Dictionary<string, object>>> GetStockIndicatorsAsync(string symbol, List<string> indicators, string interval = "daily", int timePeriod = 14, int limit = 100);
        Task<GlobalQuote> GetGlobalQuoteAsync(string symbol);
    }
}

