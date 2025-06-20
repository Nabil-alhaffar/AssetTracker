using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the side in trading.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TradeSide
    {
        /// <summary>
        /// Represents buying shares.
        /// </summary>
        [EnumMember(Value = "BUY")]
        Buy,

        /// <summary>
        /// Represents selling shares .
        /// </summary>
        [EnumMember(Value = "SELL")]
        Sell,


    }
}