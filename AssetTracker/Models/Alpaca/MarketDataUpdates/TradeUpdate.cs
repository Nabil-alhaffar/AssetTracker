using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.MarketDataUpdates
{
    /// <summary>
    /// Represents a real-time trade update message containing executed trade details for a stock symbol, as provided by Alpaca.
    /// </summary>
    public sealed record TradeUpdate
    {
        /// <summary>
        /// Gets or sets the type of update (typically "t" for trade).
        /// </summary>
        [JsonPropertyName("T")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the stock symbol for which the trade occurred.
        /// </summary>
        [JsonPropertyName("S")]
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the executed trade price.
        /// </summary>
        [JsonPropertyName("p")]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the number of shares traded.
        /// </summary>
        [JsonPropertyName("s")]
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the trade execution.
        /// </summary>
        [JsonPropertyName("t")]
        public DateTime Timestamp { get; set; }
    }
}