using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the request payload for logging out a user.
    /// </summary>
    public sealed record LogoutRequest
    {
        /// <summary>
        /// The unique session ID associated with the user's session to be ended.
        /// </summary>
        public string SessionId { get; set; } = null!;
    }
}