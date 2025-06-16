using System;
using AssetTracker.Models;

namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Interface for interacting with the Alpha Vantage stock market data API.
    /// </summary>
    public interface IAlphaVantageStockMarketService
    {
        /// <summary>
        /// Retrieves the current stock price for a given symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol (e.g., AAPL).</param>
        /// <returns>The current price of the stock as a decimal.</returns>
        Task<decimal> GetStockPriceAsync(string symbol);

        /// <summary>
        /// Retrieves an overview of the stock, including fundamental information.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A <see cref="Stock"/> object containing overview details.</returns>
        Task<Stock> GetStockOverviewAsync(string symbol);

        /// <summary>
        /// Generates a company logo URL based on the company's website.
        /// </summary>
        /// <param name="website">The website URL of the company (e.g., "apple.com").</param>
        /// <returns>A URL string pointing to the company's logo image.</returns>
        string GetCompanyLogoUrl(string website);

        /// <summary>
        /// Retrieves historical stock data for a specified symbol and interval.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <param name="interval">The time interval (e.g., "1min", "5min", "daily").</param>
        /// <returns>A collection of <see cref="HistoricalData"/> objects.</returns>
        Task<IEnumerable<HistoricalData>> GetHistoricalDataAsync(string symbol, string interval);

        /// <summary>
        /// Retrieves various technical indicators for a stock.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <param name="indicators">A list of indicator names (e.g., "RSI", "SMA").</param>
        /// <param name="interval">The time interval (e.g., "daily").</param>
        /// <param name="timePeriod">The time period to calculate the indicators.</param>
        /// <param name="limit">Maximum number of data points to retrieve.</param>
        /// <returns>
        /// A dictionary where each indicator maps to a dictionary of timestamped values.
        /// </returns>
        Task<Dictionary<string, Dictionary<string, object>>> GetStockIndicatorsAsync(string symbol, List<string> indicators, string interval = "daily", int timePeriod = 14, int limit = 100);

        /// <summary>
        /// Retrieves the latest global quote for a given stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A <see cref="GlobalQuote"/> object with real-time market data.</returns>
        Task<GlobalQuote> GetGlobalQuoteAsync(string symbol);
    }
}