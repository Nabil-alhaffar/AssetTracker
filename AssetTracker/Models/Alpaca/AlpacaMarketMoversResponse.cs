using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models
{
	public class AlpacaMarketMoversResponse
	{
		[JsonPropertyName("gainers")]
		public List<AlpacaMarketMoversItem> Gainers { get; set; }

        [JsonPropertyName("losers")]
        public List <AlpacaMarketMoversItem> Losers { get; set; }


        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }
	}
}

