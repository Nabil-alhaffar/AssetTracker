using System;
using AssetTracker.Models;

namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Interface for handling authentication and token generation.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user by verifying the username and password.
        /// </summary>
        /// <param name="username">The username of the user attempting to authenticate.</param>
        /// <param name="password">The password associated with the username.</param>
        /// <returns>The authenticated <see cref="User"/> object if credentials are valid.</returns>
        Task<User> AuthenticateUserAsync(string username, string password);

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user to generate the token for.</param>
        /// <returns>A signed JWT token string.</returns>
        string GenerateJwtToken(User user);

        /// <summary>
        /// Generates a secure refresh token for renewing access tokens.
        /// </summary>
        /// <returns>A new refresh token string.</returns>
        string GenerateRefreshToken();
    }
}