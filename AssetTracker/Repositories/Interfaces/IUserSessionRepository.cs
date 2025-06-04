using System;
using AssetTracker.Models;

namespace AssetTracker.Repositories.Interfaces
{
    public interface IUserSessionRepository
    {
        Task SaveSessionAsync(UserSession session);
        Task EndSessionAsync(string sessionId);
    }
}

