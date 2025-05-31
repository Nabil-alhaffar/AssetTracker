using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Finnhub
{
    public class EarningsItem
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("hour")]
        public string Hour { get; set; }

        [JsonPropertyName("epsEstimate")]
        public decimal? EpsEstimate { get; set; }

        [JsonPropertyName("epsActual")]
        public decimal? EpsActual { get; set; }

        [JsonPropertyName("revenueEstimate")]
        public decimal? RevenueEstimate { get; set; }

        [JsonPropertyName("revenueActual")]
        public decimal? RevenueActual { get; set; }

        [JsonPropertyName("quarter")]
        public int Quarter { get; set; }
         

    }

}