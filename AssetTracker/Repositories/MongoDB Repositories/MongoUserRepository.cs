using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Models;
using MongoDB.Driver;
using AssetTracker.Repositories.Interfaces;

namespace AssetTracker.Repositories.MongoDBRepositories
{
    /// <summary>
    /// MongoDB implementation of the <see cref="IUserRepository"/> interface.
    /// Provides methods to manage user records in the MongoDB database.
    /// </summary>
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _userCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoUserRepository"/> class using the provided MongoDB database.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        public MongoUserRepository(IMongoDatabase database)
        {
            _userCollection = database.GetCollection<User>("Users");
        }

        /// <summary>
        /// Adds a new user to the collection.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <exception cref="InvalidOperationException">Thrown if the user already exists.</exception>
        public async Task AddUserAsync(User user)
        {
            var existingUser = await _userCollection.Find(u => u.UserId == user.UserId).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists.");
            }

            await _userCollection.InsertOneAsync(user);
        }

        /// <summary>
        /// Retrieves a user by their user ID.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>The matching <see cref="User"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the user is not found.</exception>
        public async Task<User> GetUserByIDAsync(Guid userId)
        {
            var user = await _userCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"User: {userId} not found.");
            }
            return user;
        }

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <returns>The matching <see cref="User"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the user is not found.</exception>
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = await _userCollection.Find(u => u.UserName == username).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            return user;
        }

        /// <summary>
        /// Retrieves a user by their email.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>The matching <see cref="User"/> if found; otherwise, null.</returns>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
            return user;
        }

        /// <summary>
        /// Retrieves all users in the collection.
        /// </summary>
        /// <returns>An enumerable of all users.</returns>
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = await _userCollection.Find(_ => true).ToListAsync();
            return users;
        }

        /// <summary>
        /// Removes a user by their user ID.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <exception cref="InvalidOperationException">Thrown if the user is not found.</exception>
        public async Task RemoveUserAsync(Guid userId)
        {
            var result = await _userCollection.DeleteOneAsync(u => u.UserId == userId);
            if (result.DeletedCount == 0)
            {
                throw new InvalidOperationException("User not found.");
            }
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user object with updated data.</param>
        /// <exception cref="InvalidOperationException">Thrown if the user is not found.</exception>
        public async Task UpdateUserAsync(User user)
        {
            try
            {
                Console.WriteLine($"Attempting to update user with UserId: {user.UserId}");
                
                var result = await _userCollection.ReplaceOneAsync(
                    u => u.UserId == user.UserId,
                    user,
                    new ReplaceOptions { IsUpsert = false }
                );

                Console.WriteLine($"Update result - MatchedCount: {result.MatchedCount}, ModifiedCount: {result.ModifiedCount}");

                if (result.MatchedCount == 0)
                {
                    throw new InvalidOperationException($"User with ID {user.UserId} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateUserAsync for UserId {user.UserId}: {ex.Message}");
                throw;
            }
        }
    }
}