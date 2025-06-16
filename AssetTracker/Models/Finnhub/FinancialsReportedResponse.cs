using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents the response containing a list of reported financial filings.
    /// </summary>
    public sealed record FinancialsReportedResponse
    {
        /// <summary>
        /// The list of financial report filings included in the response.
        /// </summary>
        [JsonPropertyName("data")]
        public List<FinancialReportFiling> Data { get; set; } = new();
    }
}