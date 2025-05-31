using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AssetTracker.Models.Finnhub
{
	public class FinancialMetric
	{
        [JsonPropertyName("concept")]
        public string Concept { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        [JsonPropertyName("value")]
        public decimal? Value { get; set; }
    }
}

