using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents a financial report filing with metadata and the financial report details.
    /// </summary>
    public sealed record FinancialReportFiling
    {
        /// <summary>
        /// The stock symbol associated with this filing.
        /// </summary>
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// Date when the filing was accepted.
        /// </summary>
        [JsonPropertyName("acceptedDate")]
        public string? AcceptedDate { get; set; }

        /// <summary>
        /// Date when the filing was submitted.
        /// </summary>
        [JsonPropertyName("filedDate")]
        public string? FiledDate { get; set; }

        /// <summary>
        /// Start date of the reporting period.
        /// </summary>
        [JsonPropertyName("startDate")]
        public string? StartDate { get; set; }

        /// <summary>
        /// End date of the reporting period.
        /// </summary>
        [JsonPropertyName("endDate")]
        public string? EndDate { get; set; }

        /// <summary>
        /// Central Index Key (CIK) for the filing entity.
        /// </summary>
        [JsonPropertyName("cik")]
        public string? CIK { get; set; }

        /// <summary>
        /// Access number of the filing.
        /// </summary>
        [JsonPropertyName("accessNumber")]
        public string? AccessNumber { get; set; }

        /// <summary>
        /// Form type of the filing.
        /// </summary>
        [JsonPropertyName("form")]
        public string? Form { get; set; }

        /// <summary>
        /// The detailed financial report contained in this filing.
        /// </summary>
        [JsonPropertyName("report")]
        public FinancialReport Report { get; set; } = null!;

        /// <summary>
        /// Fiscal quarter of the filing.
        /// </summary>
        [JsonPropertyName("quarter")]
        public int? Quarter { get; set; }

        /// <summary>
        /// Fiscal year of the filing.
        /// </summary>
        [JsonPropertyName("year")]
        public int? Year { get; set; }
    }
}