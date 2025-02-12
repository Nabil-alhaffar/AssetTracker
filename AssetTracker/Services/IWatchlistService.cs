using System;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services
{

    public interface IWatchlistService
    {

        public Task<List<Watchlist>> GetUserWatchlistsAsync(string userId);

        public Task AddWatchlistAsync(string userId, Watchlist watchlist);

        public Task RemoveWatchlistAsync(string userId, string watchlistId);

        public Task AddSymbolToWatchlistAsync(string userId, string watchlistId, string symbol);

        public Task RemoveSymbolFromWatchlistAsync(string userId, string watchlistId, string symbol);
    }
}


