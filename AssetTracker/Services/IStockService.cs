using AssetTracker.Models;
using Microsoft.AspNetCore.Mvc;

public interface IStockService
{
    Task<double> GetStockPriceAsync(string symbol);
    Task<Stock> GetStockOverviewAsync(string symbol);
    string GetCompanyLogoUrl(string website);
    Task<IEnumerable<HistoricalData>> GetHistoricalDataAsync(string symbol, string interval);
}