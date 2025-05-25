using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models
{
    public class LoginModel
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}

