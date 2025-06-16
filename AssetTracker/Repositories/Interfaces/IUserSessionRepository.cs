using System;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Repositories.Interfaces
{
    /// <summary>
    /// Interface for user session data access operations.
    /// </summary>
    public interface IUserSessionRepository
    {
        /// <summary>
        /// Saves a new user session asynchronously.
        /// </summary>
        /// <param name="session">The user session to save.</param>
        Task SaveSessionAsync(UserSession session);

        /// <summary>
        /// Ends a user session asynchronously by session ID.
        /// </summary>
        /// <param name="sessionId">The session identifier to end.</param>
        Task EndSessionAsync(string sessionId);
    }
}