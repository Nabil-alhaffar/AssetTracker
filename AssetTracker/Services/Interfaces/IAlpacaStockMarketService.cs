using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Models.Alpaca;

namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Defines methods for interacting with Alpaca stock market data and services.
    /// </summary>
    public interface IAlpacaStockMarketService
    {
        /// <summary>
        /// Retrieves market snapshot data for a list of symbols.
        /// </summary>
        /// <param name="symbols">List of stock symbols to retrieve snapshots for.</param>
        /// <returns>JSON string containing snapshot data.</returns>
        Task<string> GetSnapshotsAsync(List<string> symbols);

        /// <summary>
        /// Retrieves recent news items for a given stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol to retrieve news for.</param>
        /// <param name="limit">The maximum number of news items to retrieve (default is 20).</param>
        /// <returns>List of news items related to the stock symbol.</returns>
        Task<List<AlpacaNewsItem>> GetNewsAsync(string symbol, int limit = 20);

        /// <summary>
        /// Retrieves historical bar data for a given stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol to get historical data for.</param>
        /// <param name="timeframe">Timeframe for the bars (e.g., "1Day", "1Min").</param>
        /// <param name="start">Start date for historical data in YYYY-MM-DD format.</param>
        /// <returns>Response containing historical bar data.</returns>
        Task<AlpacaBarsResponse> GetHistoricalBarsAsync(string symbol, string timeframe = "1Day", string start = "2024-01-01", int limit = 1000);

        /// <summary>
        /// Retrieves a list of the most active stocks on the market.
        /// </summary>
        /// <returns>Response containing the most active stocks.</returns>
        Task<AlpacaMostActiveResponse> GetMostActivesAsync();

        /// <summary>
        /// Retrieves top market movers based on the specified market type.
        /// </summary>
        /// <param name="marketType">The type of market movers to retrieve (e.g., "gainers", "losers").</param>
        /// <returns>Response containing market movers.</returns>
        Task<AlpacaMarketMoversResponse> GetMarketMoversAsync(string marketType);

        /// <summary>
        /// Searches for symbols matching a given query string.
        /// </summary>
        /// <param name="query">The search query (e.g., partial or full stock symbol or company name).</param>
        /// <returns>List of matching symbol lookup results.</returns>
        Task<List<SymbolLookupResult>> SearchAsync(string query);

        /// <summary>
        /// Initializes the Alpaca service (e.g., authenticates, establishes connections).
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task InitializeAsync();


        public Task<AlpacaAsset> GetAssetBySymbolAsync(string symbol);

    }
}