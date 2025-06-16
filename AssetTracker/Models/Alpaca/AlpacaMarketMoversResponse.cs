using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a response containing market movers from Alpaca,
    /// including top gainers, top losers, and the last update timestamp.
    /// </summary>
    public class AlpacaMarketMoversResponse
    {
        /// <summary>
        /// Gets or sets the list of top gaining stocks.
        /// </summary>
        [JsonPropertyName("gainers")]
        public List<AlpacaMarketMoversItem> Gainers { get; set; } = null!;

        /// <summary>
        /// Gets or sets the list of top losing stocks.
        /// </summary>
        [JsonPropertyName("losers")]
        public List<AlpacaMarketMoversItem> Losers { get; set; } = null!;

        /// <summary>
        /// Gets or sets the timestamp when the market movers data was last updated.
        /// </summary>
        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }
    }
}