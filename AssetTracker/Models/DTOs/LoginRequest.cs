using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models
{
    public class LoginRequest
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; } = null!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        public string? TimeZoneId { get; set; }
    }
}

