using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the request payload for user registration.
    /// </summary>
    public sealed record RegisterRequest
    {
        /// <summary>
        /// The username chosen by the user.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// The first name of the user.
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// The last name of the user.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// The email address of the user.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// The password set by the user.
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// The time zone ID specified by the user (optional).
        /// </summary>
        public string? TimeZoneId { get; set; }
    }
}