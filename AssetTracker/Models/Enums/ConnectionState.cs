using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the current state of a connection.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum ConnectionState
    {
        /// <summary>
        /// The connection is stopped and inactive.
        /// </summary>
        Stopped,

        /// <summary>
        /// The connection is in the process of starting.
        /// </summary>
        Starting,

        /// <summary>
        /// The connection is currently running and active.
        /// </summary>
        Running,

        /// <summary>
        /// The connection is in the process of stopping.
        /// </summary>
        Stopping
    }
}