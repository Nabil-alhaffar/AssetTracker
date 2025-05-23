using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models
{
	public class AlpacaMostActiveItem
	{
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("trade_count")]
        public int TradeCount { get; set; }

        [JsonPropertyName("volume")]
        public long Volume { get; set; }
    }
}

