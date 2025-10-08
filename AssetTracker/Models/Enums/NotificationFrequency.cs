using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the frequency of notifications.
    /// </summary>
    ///
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum NotificationFrequency
    {
        Immediate,
        Daily,
        Weekly
    }
}

