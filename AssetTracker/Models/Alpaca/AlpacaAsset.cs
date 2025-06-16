using System;

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
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the full name of the asset (e.g., Apple Inc.).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the class of the asset (e.g., us_equity).
        /// </summary>
        public string AssetClass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the asset is currently tradable.
        /// </summary>
        public bool Tradable { get; set; }
    }
}