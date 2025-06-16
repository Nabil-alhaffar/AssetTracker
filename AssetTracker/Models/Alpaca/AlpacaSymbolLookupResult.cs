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
    }
}