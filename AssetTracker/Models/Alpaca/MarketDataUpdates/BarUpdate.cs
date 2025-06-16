// BarUpdate.cs
using System.Text.Json.Serialization;

/// <summary>
/// Represents a real-time bar update for a stock symbol, including OHLC data and volume.
/// </summary>
public sealed record BarUpdate
{
    /// <summary>
    /// Gets or sets the type of update (typically "b" for bar).
    /// </summary>
    [JsonPropertyName("T")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the stock symbol associated with this update.
    /// </summary>
    [JsonPropertyName("S")]
    public string Symbol { get; set; }

    /// <summary>
    /// Gets or sets the opening price of the bar.
    /// </summary>
    [JsonPropertyName("o")]
    public decimal Open { get; set; }

    /// <summary>
    /// Gets or sets the highest price of the bar.
    /// </summary>
    [JsonPropertyName("h")]
    public decimal High { get; set; }

    /// <summary>
    /// Gets or sets the lowest price of the bar.
    /// </summary>
    [JsonPropertyName("l")]
    public decimal Low { get; set; }

    /// <summary>
    /// Gets or sets the closing price of the bar.
    /// </summary>
    [JsonPropertyName("c")]
    public decimal Close { get; set; }

    /// <summary>
    /// Gets or sets the trading volume for the bar.
    /// </summary>
    [JsonPropertyName("v")]
    public long Volume { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the bar update.
    /// </summary>
    [JsonPropertyName("t")]
    public DateTime Timestamp { get; set; }
}