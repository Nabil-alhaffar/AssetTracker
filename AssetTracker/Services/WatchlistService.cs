using System;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services
{
    public class WatchlistService:IWatchlistService
    {
        private readonly IWatchlistRepository _repository;

        public WatchlistService(IWatchlistRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Watchlist>> GetUserWatchlistsAsync(string userId) =>
            await _repository.GetUserWatchlistsAsync(userId);

        public async Task AddWatchlistAsync(string userId, Watchlist watchlist)
        {
            if (watchlist.Symbols.Count > 50)
                throw new Exception("A watchlist cannot have more than 50 stocks.");

            await _repository.AddWatchlistAsync(userId, watchlist);
        }

        public async Task RemoveWatchlistAsync(string userId, string watchlistId) =>
            await _repository.RemoveWatchlistAsync(userId, watchlistId);

        public async Task AddSymbolToWatchlistAsync(string userId, string watchlistId, string symbol)
        {
            var watchlists = await _repository.GetUserWatchlistsAsync(userId);
            var watchlist = watchlists.Find(w => w.Id == watchlistId);

            if (watchlist == null)
                throw new Exception("Watchlist not found");

            if (watchlist.Symbols.Count >= 50)
                throw new Exception("A watchlist cannot have more than 50 stocks.");

            await _repository.AddSymbolToWatchlistAsync(userId, watchlistId, symbol);
        }

        public async Task RemoveSymbolFromWatchlistAsync(string userId, string watchlistId, string symbol) =>
            await _repository.RemoveSymbolFromWatchlistAsync(userId, watchlistId, symbol);
    }

}

