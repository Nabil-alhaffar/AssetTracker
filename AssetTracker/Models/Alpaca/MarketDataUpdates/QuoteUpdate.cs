using System;

namespace AssetTracker.Models.MarketDataUpdates
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a real-time quote update containing bid/ask information for a stock symbol.
    /// </summary>
    public sealed record QuoteUpdate
    {
        /// <summary>
        /// Gets or sets the type of the update (typically "q" for quote).
        /// </summary>
        [JsonPropertyName("T")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the stock symbol associated with this quote update.
        /// </summary>
        [JsonPropertyName("S")]
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the ask price (lowest price a seller is willing to accept).
        /// </summary>
        [JsonPropertyName("ap")]
        public decimal AskPrice { get; set; }

        /// <summary>
        /// Gets or sets the bid price (highest price a buyer is willing to pay).
        /// </summary>
        [JsonPropertyName("bp")]
        public decimal BidPrice { get; set; }

        /// <summary>
        /// Gets or sets the ask size (number of shares available at the ask price).
        /// </summary>
        [JsonPropertyName("as")]
        public int AskSize { get; set; }

        /// <summary>
        /// Gets or sets the bid size (number of shares available at the bid price).
        /// </summary>
        [JsonPropertyName("bs")]
        public int BidSize { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of when the quote update was published.
        /// </summary>
        [JsonPropertyName("t")]
        public DateTime Timestamp { get; set; }
    }
}