using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AlertType
    {
        PriceAbove,
        PriceBelow,
        SMA,
        EMA,
        MACD,
        RSI,
        BollingerBands,
        Price,
        PercentageChange,
        Volume
    }
}

