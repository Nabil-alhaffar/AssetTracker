using System;

namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents the recommendation trend for a given stock symbol over a specific period.
    /// </summary>
    public class RecommendationTrend
    {
        /// <summary>
        /// Gets or sets the stock symbol.
        /// </summary>
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// Gets or sets the period for the recommendation trend (e.g., quarter or date range).
        /// </summary>
        public string Period { get; set; } = null!;

        /// <summary>
        /// Gets or sets the count of strong buy recommendations.
        /// </summary>
        public int StrongBuy { get; set; }

        /// <summary>
        /// Gets or sets the count of buy recommendations.
        /// </summary>
        public int Buy { get; set; }

        /// <summary>
        /// Gets or sets the count of hold recommendations.
        /// </summary>
        public int Hold { get; set; }

        /// <summary>
        /// Gets or sets the count of sell recommendations.
        /// </summary>
        public int Sell { get; set; }

        /// <summary>
        /// Gets or sets the count of strong sell recommendations.
        /// </summary>
        public int StrongSell { get; set; }
    }
}