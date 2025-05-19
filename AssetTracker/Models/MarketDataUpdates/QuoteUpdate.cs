using System;
namespace AssetTracker.Models.MarketDataUpdates
{
    using System.Text.Json.Serialization;

    public sealed record QuoteUpdate
    {
        [JsonPropertyName("T")]
        public string Type { get; set; }

        [JsonPropertyName("S")]
        public string Symbol { get; set; }

        [JsonPropertyName("ap")]
        public decimal AskPrice { get; set; }

        [JsonPropertyName("bp")]
        public decimal BidPrice { get; set; }

        [JsonPropertyName("as")]
        public int AskSize { get; set; }

        [JsonPropertyName("bs")]
        public int BidSize { get; set; }

        [JsonPropertyName("t")]
        public DateTime Timestamp { get; set; }
    }
}

