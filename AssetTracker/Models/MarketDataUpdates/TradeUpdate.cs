using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.MarketDataUpdates
{
    // TradeUpdate.cs

    public sealed record TradeUpdate
    {
        [JsonPropertyName("T")]
        public string Type { get; set; }

        [JsonPropertyName("S")]
        public string Symbol { get; set; }

        [JsonPropertyName("p")]
        public decimal Price { get; set; }

        [JsonPropertyName("s")]
        public int Size { get; set; }

        [JsonPropertyName("t")]
        public DateTime Timestamp { get; set; }
    }

}

