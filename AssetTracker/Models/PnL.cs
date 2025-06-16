using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents profit and loss data.
    /// </summary>
    public sealed record PnL
    {
        /// <summary>
        /// Gets or sets the absolute value of profit or loss.
        /// </summary>
        public decimal PNLValue { get; set; }

        /// <summary>
        /// Gets or sets the percentage of profit or loss.
        /// </summary>
        public decimal PNLPercentage { get; set; }
    }
}