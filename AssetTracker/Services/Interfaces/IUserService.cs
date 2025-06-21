using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Provides user management and authentication operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddUserAsync(User user);

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A task that returns the user.</returns>
        Task<User> GetUserAsync(Guid userId);

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A task that returns a collection of users.</returns>
        Task<IEnumerable<User>> GetUsersAsync();

        /// <summary>
        /// Removes a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveUsersAsync(Guid userId);

        /// <summary>
        /// Authenticates a user using their username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>A task that returns the authenticated user.</returns>
        Task<User> AuthenticateUserAsync(string username, string password);

        /// <summary>
        /// Registers a new user with the specified password.
        /// </summary>
        /// <param name="user">The user to register.</param>
        /// <param name="password">The password.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RegisterUserAsync(User user, string password);

        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ResetPasswordAsync(Guid userId, string newPassword);

        /// <summary>
        /// Resets the email for a user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="newEmail">The new email address.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ResetEmailAsync(Guid userId, string newEmail);

        /// <summary>
        /// Resets the username for a user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="newUsername">The new username.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ResetUsernameAsync(Guid userId, string newUsername);

        /// <summary>
        /// Clears the refresh token for a user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ClearRefreshTokenAsync(Guid userId);

        /// <summary>
        /// Updates the user's refresh token and its expiry time.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="refreshToken">The new refresh token.</param>
        /// <param name="refreshTokenExpiryTime">The expiry time of the refresh token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateUserRefreshTokenAsync(Guid userId, string refreshToken, DateTime refreshTokenExpiryTime);

        /// <summary>
        /// Updates the user's time zone.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="timeZoneId">The new time zone identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateUserTimeZoneAsync(Guid userId, string timeZoneId);

        /// <summary>
        /// Updates a user's information.
        /// </summary>
        /// <param name="user">The updated user object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateUserAsync(User user);
    }
}