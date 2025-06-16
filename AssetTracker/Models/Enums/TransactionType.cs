using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        [EnumMember(Value = "DEPOSIT")]
        Deposit,

        [EnumMember(Value = "WITHDRAWAL")]
        Withdrawal,

        [EnumMember(Value = "TRANSFER")]
        Transfer,

        [EnumMember(Value = "FEE")]
        Fee,

        [EnumMember(Value = "INTEREST")]
        Interest,

        [EnumMember(Value = "ADJUSTMENT")]
        Adjustment,
    }
}

