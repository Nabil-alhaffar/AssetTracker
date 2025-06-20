using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the trader's intent with the order.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TradeIntent
    {
        /// <summary>
        /// Buy to open a long position.
        /// </summary>
        [EnumMember(Value = "BUY_TO_OPEN")]
        BuyToOpen,

        /// <summary>
        /// Buy to close an existing short position.
        /// </summary>
        [EnumMember(Value = "BUY_TO_CLOSE")]
        BuyToClose,

        /// <summary>
        /// Sell to open a short position.
        /// </summary>
        [EnumMember(Value = "SELL_TO_OPEN")]
        SellToOpen,

        /// <summary>
        /// Sell to close a long position.
        /// </summary>
        [EnumMember(Value = "SELL_TO_CLOSE")]
        SellToClose
    }

}

