using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents an Alpaca asset, including metadata such as symbol, name, class, and tradability.
    /// </summary>
    public class AlpacaAsset
    {
        /// <summary>
        /// Gets or sets the stock or asset symbol (e.g., AAPL, TSLA).
        /// </summary>
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// Gets or sets the full name of the asset (e.g., Apple Inc.).
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the class of the asset (e.g., us_equity).
        /// </summary>
        [JsonPropertyName("class")]
        public string AssetClass { get; set; } = null!;


        /// <summary>
        /// Gets or sets the exchange of the asset (e.g., NASDAQ, AMEX).
        /// </summary>
        [JsonPropertyName("exchange")]
        public string Exchange { get; set; } = null!;

        /// <summary>
        /// Gets or sets a value indicating whether the asset is currently tradable.
        /// </summary>
        public bool Tradable { get; set; }
    }
}