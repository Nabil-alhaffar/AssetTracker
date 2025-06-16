using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a request to reset or update the user's password.
    /// </summary>
    public sealed record ResetPasswordRequest
    {
        /// <summary>
        /// The new password to set for the user.
        /// </summary>
        public string NewPassword { get; set; } = null!;
    }
}