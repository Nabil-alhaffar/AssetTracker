using System;
namespace AssetTracker.Services.Interfaces
{
    public interface IUserSessionManager
    {
        public Task StartSessionAsync(Guid userId, string sessionId, string ipAddress = null, string userAgent = null);
        Task EndSessionAsync(Guid userId, string sessionId);
        Task<string?> GetSessionIdAsync(Guid userId);
        Task<bool> IsSessionValidAsync(Guid userId, string sessionId);
    }

}

