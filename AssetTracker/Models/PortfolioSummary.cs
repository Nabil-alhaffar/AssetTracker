using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a summary of the user's portfolio including market value, costs, and performance metrics.
    /// </summary>
    public sealed record PortfolioSummary
    {
        /// <summary>
        /// Gets or sets the total current market value of all holdings in the portfolio.
        /// </summary>
        public decimal MarketValue { get; set; }

        /// <summary>
        /// Gets or sets the total cost basis of all holdings in the portfolio.
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets the net account value, including cash and holdings.
        /// </summary>
        public decimal NetAccountValue { get; set; }

        /// <summary>
        /// Gets or sets the current cash balance available in the account.
        /// </summary>
        public decimal CashBalance { get; set; }

        /// <summary>
        /// Gets or sets the profit or loss realized during the current day.
        /// </summary>
        public decimal DayPNL { get; set; }

        /// <summary>
        /// Gets or sets the return percentage for the current day.
        /// </summary>
        public decimal DayReturnPercentage { get; set; }

        /// <summary>
        /// Gets or sets the unrealized profit or loss of open positions.
        /// </summary>
        public decimal OpenPNL { get; set; }

        /// <summary>
        /// Gets or sets the return percentage of the open positions.
        /// </summary>
        public decimal OpenReturnPercentage { get; set; }

        /// <summary>
        /// Gets or sets the current margin used by open positions.
        /// </summary>
        public decimal MarginUsed { get; set; }

        /// <summary>
        /// Gets or sets the maximum margin limit available.
        /// </summary>
        public decimal MarginLimit { get; set; }

        /// <summary>
        /// Gets or sets the available buying power (cash + unused margin).
        /// </summary>
        public decimal BuyingPower { get; set; }

        /// <summary>
        /// Gets or sets the user's equity (cash + long positions - short positions).
        /// </summary>
        public decimal Equity { get; set; }

        /// <summary>
        /// Gets or sets whether the user is currently in a margin call.
        /// </summary>
        public bool IsInMarginCall { get; set; }

        /// <summary>
        /// Gets or sets the maintenance margin requirement as a percentage.
        /// </summary>
        public decimal MaintenanceMarginRequirement { get; set; }

        /// <summary>
        /// Gets or sets the initial margin requirement as a percentage.
        /// </summary>
        public decimal InitialMarginRequirement { get; set; }
    }
}