using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Models;
using MongoDB.Driver;
using AssetTracker.Repositories.Interfaces;

namespace AssetTracker.Repositories.MongoDBRepositories
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _userCollection;

        // Constructor initializes the MongoDB collection
        public MongoUserRepository(IMongoDatabase database)
        {
            _userCollection = database.GetCollection<User>("Users");  // 'Users' is the collection name
        }

        // Add a user to the MongoDB collection
        public async Task AddUserAsync(User user)
        {
            // Check if the user already exists
            var existingUser = await _userCollection.Find(u => u.UserId == user.UserId).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists.");
            }

            await _userCollection.InsertOneAsync(user);  // Insert the new user
        }

        // Retrieve a user by their UserId
        public async Task<User> GetUserByIDAsync(Guid userId)
        {
            var user = await _userCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"User: {userId} not found.");
            }
            return user;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = await _userCollection.Find(u => u.UserName == username).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            return user;
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            return user;
        }

        // Retrieve all users
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = await _userCollection.Find(_ => true).ToListAsync();  // Fetch all users
            return users;
        }

        // Remove a user by their UserId
        public async Task RemoveUserAsync(Guid userId)
        {
            var result = await _userCollection.DeleteOneAsync(u => u.UserId == userId);
            if (result.DeletedCount == 0)
            {
                throw new InvalidOperationException("User not found.");
            }
        }

        // Update an existing user
        public async Task UpdateUserAsync(User user)
        {
            var result = await _userCollection.ReplaceOneAsync(
                u => u.UserId == user.UserId,
                user,
                new ReplaceOptions { IsUpsert = false }
            );

            if (result.MatchedCount == 0)
            {
                throw new InvalidOperationException("User not found.");
            }
        }
    }
}
