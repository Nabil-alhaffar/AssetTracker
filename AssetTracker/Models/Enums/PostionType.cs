using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum PositionType
    {
        [EnumMember(Value = "LONG")]
        Long,

        [EnumMember(Value = "SHORT")]
        Short,

    }
}

