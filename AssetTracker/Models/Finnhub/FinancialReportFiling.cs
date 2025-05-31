using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AssetTracker.Models.Finnhub
{
	public class FinancialReportFiling
	{
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("reportDate")]
        public string? ReportDate { get; set; }

        [JsonPropertyName("filingDate")]
        public string? FilingDate { get; set; }

        [JsonPropertyName("accessNumber")]
        public string? AccessNumber { get; set; }

        [JsonPropertyName("form")]
        public string? Form { get; set; }

        [JsonPropertyName("report")]
        public FinancialReport Report { get; set; }
    }

}

