using System;
using AssetTracker.Models;
using AssetTracker.Repositories.Interfaces;
using MongoDB.Driver;

namespace AssetTracker.Repositories.MongoDBRepositories
{
    /// <summary>
    /// MongoDB implementation of <see cref="IUserSessionRepository"/> interface.
    /// Provides CRUD operations for user sessions stored in MongoDB.
    /// </summary>
    public class MongoUserSessionRepository : IUserSessionRepository
    {
        private readonly IMongoCollection<UserSession> _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoUserSessionRepository"/> class.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        public MongoUserSessionRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<UserSession>("UserSessions");
        }

        /// <summary>
        /// Saves a new user session to the database.
        /// </summary>
        /// <param name="session">The user session to save.</param>
        public async Task SaveSessionAsync(UserSession session)
        {
            await _collection.InsertOneAsync(session);
        }

        /// <summary>
        /// Ends a user session by setting its EndedAt timestamp.
        /// </summary>
        /// <param name="sessionId">The session ID of the session to end.</param>
        public async Task EndSessionAsync(string sessionId)
        {
            var update = Builders<UserSession>.Update.Set(s => s.EndedAt, DateTime.UtcNow);
            await _collection.UpdateOneAsync(s => s.SessionId == sessionId, update);
        }
    }
}