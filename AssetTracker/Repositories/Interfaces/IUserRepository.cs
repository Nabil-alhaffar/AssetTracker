using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Repositories.Interfaces
{
    /// <summary>
    /// Interface for user data access operations.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user by their unique identifier asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>The user entity.</returns>
        Task<User> GetUserByIDAsync(Guid userId);

        /// <summary>
        /// Retrieves a user by their username asynchronously.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The user entity.</returns>
        Task<User> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Retrieves a user by their email asynchronously.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>The user entity.</returns>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Retrieves all users asynchronously.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        Task<IEnumerable<User>> GetUsersAsync();

        /// <summary>
        /// Adds a new user asynchronously.
        /// </summary>
        /// <param name="user">The user to add.</param>
        Task AddUserAsync(User user);

        /// <summary>
        /// Updates an existing user asynchronously.
        /// </summary>
        /// <param name="user">The user to update.</param>
        Task UpdateUserAsync(User user);

        /// <summary>
        /// Removes a user asynchronously by their unique identifier.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        Task RemoveUserAsync(Guid userId);
    }
}