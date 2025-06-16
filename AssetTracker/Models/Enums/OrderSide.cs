using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the side or type of an order in trading.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderSide
    {
        /// <summary>
        /// Represents a buy order.
        /// </summary>
        [EnumMember(Value = "BUY")]
        Buy,

        /// <summary>
        /// Represents a sell order.
        /// </summary>
        [EnumMember(Value = "SELL")]
        Sell,

        /// <summary>
        /// Represents opening a short position.
        /// </summary>
        [EnumMember(Value = "SHORT")]
        Short,

        /// <summary>
        /// Represents closing a short position.
        /// </summary>
        [EnumMember(Value = "CLOSE_SHORT")]
        CloseShort
    }
}