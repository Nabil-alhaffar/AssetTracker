using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a single market mover item from Alpaca,
    /// typically used to show top gainers or losers.
    /// </summary>
    public class AlpacaMarketMoversItem
    {
        /// <summary>
        /// Gets or sets the stock symbol (e.g., AAPL, TSLA).
        /// </summary>
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// Gets or sets the current price of the stock.
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the absolute change in price.
        /// </summary>
        [JsonPropertyName("change")]
        public decimal Change { get; set; }

        /// <summary>
        /// Gets or sets the percent change in price.
        /// </summary>
        [JsonPropertyName("percent_change")]
        public decimal PercentChange { get; set; }
    }
}