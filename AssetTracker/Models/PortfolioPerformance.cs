using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the performance metrics of a user's portfolio.
    /// </summary>
    public sealed record PortfolioPerformance
    {
        /// <summary>
        /// Gets or sets the profit or loss value of the portfolio.
        /// </summary>
        public decimal PNL { get; set; }

        /// <summary>
        /// Gets or sets the return percentage of the portfolio relative to its initial value.
        /// </summary>
        public decimal ReturnPercentage { get; set; }
    }
}