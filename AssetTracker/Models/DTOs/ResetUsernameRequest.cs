using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a request to reset or update the user's username.
    /// </summary>
    public sealed record ResetUsernameRequest
    {
        /// <summary>
        /// The new username to set for the user.
        /// </summary>
        public string NewUsername { get; set; } = null!;
    }
}