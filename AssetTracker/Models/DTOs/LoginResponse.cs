using System;

namespace AssetTracker.Models.DTOs
{
    /// <summary>
    /// Represents the response returned upon successful login.
    /// </summary>
    public sealed record LoginResponse
    {
        /// <summary>
        /// The unique identifier of the logged-in user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The first name of the logged-in user.
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// The last name of the logged-in user.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// The email address of the logged-in user.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// The timezone ID associated with the user.
        /// </summary>
        public string? TimeZoneId { get; set; }

        /// <summary>
        /// The JWT token issued to the user for authenticated requests.
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// The unique session ID associated with the user's login session.
        /// </summary>
        public string SessionId { get; set; } = null!;

        /// <summary>
        /// A message indicating the login result.
        /// </summary>
        public string Message { get; set; } = null!;
    }
}