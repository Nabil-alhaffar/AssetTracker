using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Repositories.Interfaces;

namespace AssetTracker.Repositories.MockRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Dictionary<Guid, User> _users;

        // Constructor initializes the dictionary
        public UserRepository()
        {
            _users = new Dictionary<Guid, User>();
        }

        // Add a user to the dictionary
        public async Task AddUserAsync(User user)
        {
            if (_users.ContainsKey(user.UserId))
            {
                throw new InvalidOperationException("User already exists.");
            }

            _users[user.UserId] = user;  // Add user to dictionary using UserId as the key
            await Task.CompletedTask;  // Simulating async task (no database)
        }

        // Retrieve a user by their UserId
        public async Task<User> GetUserByIDAsync(Guid userId)
        {
            if (_users.TryGetValue(userId, out var user))
            {
                return await Task.FromResult(user);  // Return user if found
            }

            throw new InvalidOperationException("User not found.");  // Handle null case
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = _users.Values.FirstOrDefault(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user != null)
            {
                return await Task.FromResult(user);  // Return user if found
            }

            throw new InvalidOperationException("User not found.");  // Handle user not found
        }
        // Retrieve all users
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await Task.FromResult(_users.Values.ToList());  // Return all users
        }

        // Remove a user by their UserId
        public async Task RemoveUserAsync(Guid userId)
        {
            if (_users.Remove(userId))
            {
                await Task.CompletedTask;  // Successfully removed the user
            }
            else
            {
                throw new InvalidOperationException("User not found.");
            }
        }

        // Update an existing user
        public async Task UpdateUserAsync(User user)
        {
            if (_users.ContainsKey(user.UserId))
            {
                _users[user.UserId] = user;  // Update user in the dictionary
                await Task.CompletedTask;  // Simulate async task
            }
            else
            {
                throw new InvalidOperationException("User not found.");
            }
        }
    }
}
