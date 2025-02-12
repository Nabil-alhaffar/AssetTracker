using System;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services
{

    public interface IWatchlistService
    {

        public Task<List<Watchlist>> GetUserWatchlistsAsync(Guid userId);

        public Task AddWatchlistAsync(Guid userId, Watchlist watchlist);

        public Task RemoveWatchlistAsync(Guid userId, Guid watchlistId);

        public Task AddSymbolToWatchlistAsync(Guid userId, Guid watchlistId, string symbol);

        public Task RemoveSymbolFromWatchlistAsync(Guid userId, Guid watchlistId, string symbol);
    }
}


