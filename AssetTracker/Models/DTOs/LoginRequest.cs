using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the login request payload containing user credentials and optional timezone.
    /// </summary>
    public sealed record LoginRequest
    {

        /// <summary>
        /// The username of the user attempting to log in.
        /// </summary>
        [JsonPropertyName("username")]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// The password of the user attempting to log in.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        /// <summary>
        /// Optional timezone ID to associate with the user's session.
        /// </summary>
        public string? TimeZoneId { get; set; }
    }
}

