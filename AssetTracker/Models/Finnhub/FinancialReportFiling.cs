using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AssetTracker.Models.Finnhub
{
	public class FinancialReportFiling
	{
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("acceptedDate")]
        public string? AcceptedDate { get; set; }

        [JsonPropertyName("filedDate")]
        public string? FiledDate { get; set; }

        [JsonPropertyName("startDate")]
        public string? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public string? endDate { get; set; }

        [JsonPropertyName("cik")]
        public string ? CIK { get; set; }

        [JsonPropertyName("accessNumber")]
        public string? AccessNumber { get; set; }

        [JsonPropertyName("form")]
        public string? Form { get; set; }

        [JsonPropertyName("report")]
        public FinancialReport Report { get; set; }

        [JsonPropertyName("quarter")]
        public int? Quarter { get; set; }

        [JsonPropertyName("uear")]
        public int? Year { get; set; }
    }
     

      

}

