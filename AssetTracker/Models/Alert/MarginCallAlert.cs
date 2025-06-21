using System;
namespace AssetTracker.Models.Alert
{
    /// <summary>
    /// Represents a margin call alert for a user.
    /// </summary>
    public class MarginCallAlert
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current portfolio value (equity).
        /// </summary>
        public decimal PortfolioValue { get; set; }

        /// <summary>
        /// Gets or sets the margin currently used.
        /// </summary>
        public decimal MarginUsed { get; set; }

        /// <summary>
        /// Gets or sets the margin limit.
        /// </summary>
        public decimal MarginLimit { get; set; }

        /// <summary>
        /// Gets or sets the required equity to meet maintenance margin.
        /// </summary>
        public decimal RequiredEquity { get; set; }

        /// <summary>
        /// Gets or sets the shortfall amount needed to resolve the margin call.
        /// </summary>
        public decimal Shortfall { get; set; }

        /// <summary>
        /// Gets or sets when the margin call was triggered.
        /// </summary>
        public DateTime TriggeredAt { get; set; }
    }
}

