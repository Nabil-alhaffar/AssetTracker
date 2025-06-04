using System;
using AssetTracker.Models;
using AssetTracker.Repositories.Interfaces;
using MongoDB.Driver;

namespace AssetTracker.Repositories.MongoDBRepositories
{
	public class MongoUserSessionRepository: IUserSessionRepository
	{
        private readonly IMongoCollection<UserSession> _collection;

        public MongoUserSessionRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<UserSession>("UserSessions");
        }

        public async Task SaveSessionAsync(UserSession session)
        {
            await _collection.InsertOneAsync(session);
        }

        public async Task EndSessionAsync(string sessionId)
        {
            var update = Builders<UserSession>.Update.Set(s => s.EndedAt, DateTime.UtcNow);
            await _collection.UpdateOneAsync(s => s.SessionId == sessionId, update);
        }
    }
}

