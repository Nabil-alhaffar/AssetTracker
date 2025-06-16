using System;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services.Interfaces
{

    public interface IWatchlistService
    {

        public Task<List<Watchlist>> GetUserWatchlistsAsync(Guid userId);

        public Task AddWatchlistAsync(Guid userId, string watchlistName, string[]? symbols = null);

        public Task RemoveWatchlistAsync(Guid userId, Guid watchlistId);

        public Task AddSymbolsToWatchlistAsync(Guid userId, Guid watchlistId, IEnumerable<string> symbols);

        public Task RemoveSymbolsFromWatchlistAsync(Guid userId, Guid watchlistId, IEnumerable<string> symbol);

        public Task<List<string>> GetAllWatchedTickersByUserIdAsync(Guid userId);


        public Task<Guid?> GetUserWatchlistIdByName(Guid userId, string watchlistName);

    }
}


