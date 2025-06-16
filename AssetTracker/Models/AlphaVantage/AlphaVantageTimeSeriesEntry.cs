using System;
using Newtonsoft.Json;

namespace AssetTracker.Models.AlphaVantage
{
    /// <summary>
    /// Represents a single entry in a time series from the Alpha Vantage API,
    /// including open, high, low, close prices and volume for a specific timestamp.
    /// </summary>
    public class AlphaVantageTimeSeriesEntry
    {
        /// <summary>
        /// Gets or sets the opening price for the time interval.
        /// </summary>
        [JsonProperty("1. open")]
        public decimal Open { get; set; }

        /// <summary>
        /// Gets or sets the highest price for the time interval.
        /// </summary>
        [JsonProperty("2. high")]
        public decimal High { get; set; }

        /// <summary>
        /// Gets or sets the lowest price for the time interval.
        /// </summary>
        [JsonProperty("3. low")]
        public decimal Low { get; set; }

        /// <summary>
        /// Gets or sets the closing price for the time interval.
        /// </summary>
        [JsonProperty("4. close")]
        public decimal Close { get; set; }

        /// <summary>
        /// Gets or sets the volume of shares traded during the time interval.
        /// </summary>
        [JsonProperty("5. volume")]
        public long Volume { get; set; }
    }
}