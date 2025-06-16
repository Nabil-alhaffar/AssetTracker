using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Defines stock sectors.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Sector
    {
        [EnumMember(Value = "Energy")]
        Energy,

        [EnumMember(Value = "Materials")]
        Materials,

        [EnumMember(Value = "Industrials")]
        Industrials,

        [EnumMember(Value = "Consumer_Discretionary")]
        ConsumerDiscretionary,

        [EnumMember(Value = "Consumer_Staples")]
        ConsumerStaples,

        [EnumMember(Value = "Healthcare")]
        Healthcare,

        [EnumMember(Value = "Financials")]
        Financials,

        [EnumMember(Value = "Information_Technology")]
        InformationTechnology,

        [EnumMember(Value = "Communication_Services")]
        CommunicationServices,

        [EnumMember(Value = "Utilities")]
        Utilities,

        [EnumMember(Value = "Real_Estate")]
        RealEstate
    }
}

