using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a request to update a user's time zone.
    /// </summary>
    public sealed record TimeZoneUpdateRequest
    {
        /// <summary>
        /// The new time zone identifier to set (e.g., "Pacific Standard Time", "Europe/London").
        /// Must match the regex pattern of letters and slashes only.
        /// </summary>
        [Required]
        [RegularExpression(@"^[A-Za-z/_]+$", ErrorMessage = "Invalid time zone format.")]
        public string TimeZoneId { get; set; } = null!;
    }
}