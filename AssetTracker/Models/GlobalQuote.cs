using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a global quote for a stock, including pricing and trading information.
    /// </summary>
    public sealed record GlobalQuote
    {
        /// <summary>
        /// Gets or sets the stock symbol (e.g., AAPL, TSLA).
        /// </summary>
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// Gets or sets the opening price of the stock for the trading day.
        /// </summary>
        public decimal Open { get; set; }

        /// <summary>
        /// Gets or sets the highest price of the stock during the trading day.
        /// </summary>
        public decimal High { get; set; }

        /// <summary>
        /// Gets or sets the lowest price of the stock during the trading day.
        /// </summary>
        public decimal Low { get; set; }

        /// <summary>
        /// Gets or sets the last traded price of the stock.
        /// </summary>
        public decimal LastPrice { get; set; }

        /// <summary>
        /// Gets or sets the trading volume for the stock.
        /// </summary>
        public long Volume { get; set; }

        /// <summary>
        /// Gets or sets the date of the latest trading session (format: yyyy-MM-dd).
        /// </summary>
        public string LatestTradingDay { get; set; } = null!;

        /// <summary>
        /// Gets or sets the closing price of the previous trading day.
        /// </summary>
        public decimal PreviousClose { get; set; }

        /// <summary>
        /// Gets or sets the absolute change in price compared to the previous close.
        /// </summary>
        public decimal Change { get; set; }

        /// <summary>
        /// Gets or sets the percentage change in price compared to the previous close (e.g., "-1.25%").
        /// </summary>
        public string ChangePercent { get; set; } = null!;
    }
}