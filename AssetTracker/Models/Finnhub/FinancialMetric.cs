using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents a financial metric with its concept, label, unit, and value, as provided by Finnhub.
    /// </summary>
    public sealed record FinancialMetric
    {
        /// <summary>
        /// The concept or key name of the financial metric.
        /// </summary>
        [JsonPropertyName("concept")]
        public string Concept { get; set; }

        /// <summary>
        /// A human-readable label for the metric.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// The unit of measurement for the metric (e.g., USD, shares).
        /// </summary>
        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        /// <summary>
        /// The numeric value of the metric.
        /// </summary>
        [JsonPropertyName("value")]
        public decimal? Value { get; set; }
    }
}