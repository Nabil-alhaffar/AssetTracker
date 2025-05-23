using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models
{
	public class AlpacaMarketMoversItem
	{
		[JsonPropertyName ("symbol")]
		public string Symbol { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("change")]
        public decimal Change { get; set; }

        [JsonPropertyName("percent_change")]
        public decimal PercentChange { get; set; }
	}
}

