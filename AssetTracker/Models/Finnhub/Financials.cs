using System;
using System.Collections.Generic;

namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents a financial statement with metadata and associated data entries.
    /// </summary>
    public sealed record Financials
    {
        /// <summary>
        /// The stock symbol for which the financials apply.
        /// </summary>
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// The type of financial statement (e.g., Income Statement, Balance Sheet).
        /// </summary>
        public string StatementType { get; set; } = null!;

        /// <summary>
        /// The frequency of the financial statement (e.g., annual, quarterly).
        /// </summary>
        public string Frequency { get; set; } = null!;

        /// <summary>
        /// The financial data entries, where each dictionary represents a data row with key-value pairs.
        /// </summary>
        public List<Dictionary<string, string>> Data { get; set; } = new();
    }
}