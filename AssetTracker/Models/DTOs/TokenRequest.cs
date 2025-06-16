using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a request containing an access token and a refresh token for authentication purposes.
    /// </summary>
    public sealed record TokenRequest
    {
        /// <summary>
        /// The access token (JWT) that may be expired or about to expire.
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// The refresh token used to obtain a new access token.
        /// </summary>
        public string RefreshToken { get; set; } = null!;
    }
}