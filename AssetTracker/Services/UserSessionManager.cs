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

    /// <summary>
    /// Manages user sessions using Redis for fast access and MongoDB for persistent tracking.
    /// </summary>
    public class UserSessionManager : IUserSessionManager
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly IUserSessionRepository _mongoRepo;


        /// <summary>
        /// Initializes a new instance of the <see cref="UserSessionManager"/> class.
        /// </summary>
        /// <param name="redis">Redis connection multiplexer.</param>
        /// <param name="mongoRepo">MongoDB repository for session storage.</param>
        public UserSessionManager(IConnectionMultiplexer redis, IUserSessionRepository mongoRepo)
        {
            _redis = redis;
            _db = _redis.GetDatabase();
            _mongoRepo = mongoRepo;

        }

        /// <summary>
        /// Starts a new session for the specified user and stores it in Redis and MongoDB.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="sessionId">The session ID to store.</param>
        /// <param name="ipAddress">Optional IP address of the user.</param>
        /// <param name="userAgent">Optional user agent of the client.</param>
        public async Task StartSessionAsync(Guid userId, string sessionId, string ipAddress = null, string userAgent = null)
        {
            var key = $"session:{userId}";
            await _db.StringSetAsync(key, sessionId, TimeSpan.FromHours(12));

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


        /// <summary>
        /// Ends a user's session by removing it from Redis and marking it ended in MongoDB.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="sessionId">The session ID to end.</param>
        public async Task EndSessionAsync(Guid userId, string sessionId)
        {
            var key = $"session:{userId}";
            await _db.KeyDeleteAsync(key);
            await _mongoRepo.EndSessionAsync(sessionId);

        }


        /// <summary>
        /// Gets the current session ID for a user from Redis.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The session ID if found, otherwise null.</returns>
        public async Task<string?> GetSessionIdAsync(Guid userId)
        {
            var key = $"session:{userId}";
            return await _db.StringGetAsync(key);
        }

        /// <summary>
        /// Gets the current session ID for a user from Redis.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The session ID if found, otherwise null.</returns>
        public async Task<bool> IsSessionValidAsync(Guid userId, string sessionId)
        {
            var stored = await GetSessionIdAsync(userId);
            return stored == sessionId;
        }
    }


}

