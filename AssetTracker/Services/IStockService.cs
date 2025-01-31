public interface IStockService
{
    Task<double> GetStockPriceAsync(string symbol);
}