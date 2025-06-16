using System;
using System.Threading.Tasks;

namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Manages user sessions including start, end, validation, and retrieval.
    /// </summary>
    public interface IUserSessionManager
    {
        /// <summary>
        /// Starts a new session for a user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="ipAddress">Optional IP address from which the session originated.</param>
        /// <param name="userAgent">Optional user agent string.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StartSessionAsync(Guid userId, string sessionId, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Ends an existing session for a user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="sessionId">The session identifier to end.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task EndSessionAsync(Guid userId, string sessionId);

        /// <summary>
        /// Retrieves the current session ID for a user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A task that returns the session ID if exists; otherwise, null.</returns>
        Task<string?> GetSessionIdAsync(Guid userId);

        /// <summary>
        /// Checks whether a session is valid for a given user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="sessionId">The session ID to validate.</param>
        /// <returns>A task that returns true if the session is valid; otherwise, false.</returns>
        Task<bool> IsSessionValidAsync(Guid userId, string sessionId);
    }
}