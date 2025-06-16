using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderSide
    {
        [EnumMember(Value = "BUY")]
        Buy,


        [EnumMember(Value = "SELL")]
        Sell,

        [EnumMember(Value = "SHORT")]
        Short,

        [EnumMember(Value = "CLOSE_SHORT")]
        CloseShort
    }
}

