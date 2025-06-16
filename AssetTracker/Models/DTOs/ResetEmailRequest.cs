using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a request to reset or update the user's email address.
    /// </summary>
    public sealed record ResetEmailRequest
    {
        /// <summary>
        /// The new email address to set for the user.
        /// </summary>
        public string NewEmail { get; set; } = null!;
    }
}