// BarUpdate.cs
using System.Text.Json.Serialization;

public sealed record BarUpdate
{
    [JsonPropertyName("T")]
    public string Type { get; set; }

    [JsonPropertyName("S")]
    public string Symbol { get; set; }

    [JsonPropertyName("o")]
    public decimal Open { get; set; }

    [JsonPropertyName("h")]
    public decimal High { get; set; }

    [JsonPropertyName("l")]
    public decimal Low { get; set; }

    [JsonPropertyName("c")]
    public decimal Close { get; set; }

    [JsonPropertyName("v")]
    public long Volume { get; set; }

    [JsonPropertyName("t")]
    public DateTime Timestamp { get; set; }
}
