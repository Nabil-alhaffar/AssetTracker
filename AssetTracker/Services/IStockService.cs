using AssetTracker.Models;
using Microsoft.AspNetCore.Mvc;

public interface IStockService
{
    Task<double> GetStockPriceAsync(string symbol);
    Task<Stock> GetStockOverviewAsync(string symbol);
    string GetCompanyLogoUrl(string website);
    Task<IEnumerable<HistoricalData>> GetHistoricalDataAsync(string symbol, string interval);
    Task<Dictionary<string, Dictionary<string, object>>> GetStockIndicatorsAsync(string symbol, List<string> indicators, string interval = "daily", int timePeriod = 14);

}