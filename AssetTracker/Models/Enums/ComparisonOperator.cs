using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the comparison operators for alert conditions.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum ComparisonOperator
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Equal,
        NotEqual,
        Between
    }
}

