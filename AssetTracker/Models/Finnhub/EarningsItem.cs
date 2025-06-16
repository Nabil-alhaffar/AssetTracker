using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents a single earnings item in the earnings calendar.
    /// </summary>
    public sealed record EarningsItem
    {
        /// <summary>
        /// The stock symbol.
        /// </summary>
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// The date of the earnings report (usually in yyyy-MM-dd format).
        /// </summary>
        [JsonPropertyName("date")]
        public string Date { get; set; }

        /// <summary>
        /// The time of the earnings report (e.g., "bmo" for before market open).
        /// </summary>
        [JsonPropertyName("hour")]
        public string Hour { get; set; }

        /// <summary>
        /// Estimated Earnings Per Share.
        /// </summary>
        [JsonPropertyName("epsEstimate")]
        public decimal? EpsEstimate { get; set; }

        /// <summary>
        /// Actual Earnings Per Share.
        /// </summary>
        [JsonPropertyName("epsActual")]
        public decimal? EpsActual { get; set; }

        /// <summary>
        /// Estimated revenue.
        /// </summary>
        [JsonPropertyName("revenueEstimate")]
        public decimal? RevenueEstimate { get; set; }

        /// <summary>
        /// Actual revenue.
        /// </summary>
        [JsonPropertyName("revenueActual")]
        public decimal? RevenueActual { get; set; }

        /// <summary>
        /// The fiscal quarter number (1-4).
        /// </summary>
        [JsonPropertyName("quarter")]
        public int Quarter { get; set; }
    }
}