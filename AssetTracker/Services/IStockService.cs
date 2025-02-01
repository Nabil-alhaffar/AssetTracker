using AssetTracker.Models;
public interface IStockService
{
    //Task<double> GetStockPriceAsync(string symbol);
    Task<Stock> GetStockOverviewAsync(string symbol);
    string GetCompanyLogoUrl(string website);
}