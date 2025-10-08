using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Specifies the type of a trading position.
    /// </summary>
    ///

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PositionType
    {
        /// <summary>
        /// Represents a long position (buying with expectation price will rise).
        /// </summary>
        [EnumMember(Value = "LONG")]
        Long,

        /// <summary>
        /// Represents a short position (selling borrowed shares expecting price to fall).
        /// </summary>
        [EnumMember(Value = "SHORT")]
        Short,
    }
}