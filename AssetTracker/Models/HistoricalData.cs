using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a record of historical stock data for a specific trading day.
    /// </summary>
    public sealed record HistoricalData
    {
        /// <summary>
        /// Gets or sets the date of the historical data.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the closing price of the stock on the specified date.
        /// </summary>
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// Gets or sets the lowest price of the stock on the specified date.
        /// </summary>
        public decimal Low { get; set; }

        /// <summary>
        /// Gets or sets the highest price of the stock on the specified date.
        /// </summary>
        public decimal High { get; set; }

        /// <summary>
        /// Gets or sets the opening price of the stock on the specified date.
        /// </summary>
        public decimal Open { get; set; }

        /// <summary>
        /// Gets or sets the volume of shares traded on the specified date.
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoricalData"/> record.
        /// </summary>
        public HistoricalData()
        {
        }
    }
}