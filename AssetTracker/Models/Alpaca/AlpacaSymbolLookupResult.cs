using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the result of a symbol lookup operation,
    /// including the stock symbol and its corresponding company name.
    /// </summary>
    public class SymbolLookupResult
    {
        /// <summary>
        /// Gets or sets the stock symbol.
        /// </summary>
        public string? Symbol { get; set; }

        /// <summary>
        /// Gets or sets the name of the company or asset.
        /// </summary>
        public string? Name { get; set; }



        /// <summary>
        /// Gets or sets the asset class (us-equity, crypto, us-options).
        /// </summary>
        public string? AssetClass { get; set; }



        /// <summary>
        /// Gets or sets the exchange the ticker trades under.
        /// </summary>
        public string? Exchange { get; set; }
    }
}