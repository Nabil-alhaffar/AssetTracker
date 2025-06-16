using System;
using System.Collections.Generic;

namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents basic financial data metrics for a given stock symbol, as provided by Finnhub.
    /// </summary>
    public sealed record BasicFinancials
    {
        /// <summary>
        /// The stock symbol associated with these financial metrics.
        /// </summary>
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// A dictionary containing various financial metrics and their values.
        /// The keys are metric names, and the values are metric data (type can vary).
        /// </summary>
        public Dictionary<string, object> Metric { get; set; } = new Dictionary<string, object>();
    }
}