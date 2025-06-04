using System;
namespace AssetTracker.Services
{
    using AssetTracker.Models;
    using AssetTracker.Repositories.Interfaces;
    using AssetTracker.Services.Interfaces;
    using Microsoft.Extensions.Caching.Distributed;
    using StackExchange.Redis;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public class UserSessionManager : IUserSessionManager
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly IUserSessionRepository _mongoRepo;

        public UserSessionManager(IConnectionMultiplexer redis, IUserSessionRepository mongoRepo)
        {
            _redis = redis;
            _db = _redis.GetDatabase();
            _mongoRepo = mongoRepo;

        }

        public async Task StartSessionAsync(Guid userId, string sessionId, string ipAddress = null, string userAgent = null)
        {
            var key = $"session:{userId}";
            await _db.StringSetAsync(key, sessionId, TimeSpan.FromHours(12)); // or however long your session should last

            // Store in Mongo
            var session = new UserSession
            {
                UserId = userId,
                SessionId = sessionId,
                CreatedAt = DateTime.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };
            await _mongoRepo.SaveSessionAsync(session);
        }

        public async Task EndSessionAsync(Guid userId, string sessionId)
        {
            var key = $"session:{userId}";
            await _db.KeyDeleteAsync(key);
            await _mongoRepo.EndSessionAsync(sessionId);

        }

        public async Task<string?> GetSessionIdAsync(Guid userId)
        {
            var key = $"session:{userId}";
            return await _db.StringGetAsync(key);
        }

        public async Task<bool> IsSessionValidAsync(Guid userId, string sessionId)
        {
            var stored = await GetSessionIdAsync(userId);
            return stored == sessionId;
        }
    }


}

