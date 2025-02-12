using System.Collections.Generic;
using AssetTracker.Models;
using AssetTracker.Repositories;
namespace AssetTracker.Repositories
{
    public class WatchlistRepository : IWatchlistRepository
    {
        private readonly Dictionary<string, List<Watchlist>> _userWatchlists = new();

        public Task<List<Watchlist>> GetUserWatchlistsAsync(string userId) =>
            Task.FromResult(_userWatchlists.TryGetValue(userId, out var watchlists) ? watchlists : new List<Watchlist>());

        public Task AddWatchlistAsync(string userId, Watchlist watchlist)
        {
            if (!_userWatchlists.ContainsKey(userId))
                _userWatchlists[userId] = new List<Watchlist>();

            _userWatchlists[userId].Add(watchlist);
            return Task.CompletedTask; // No actual async operation here, but keeps API consistent
        }

        public Task RemoveWatchlistAsync(string userId, string watchlistId)
        {
            if (_userWatchlists.TryGetValue(userId, out var watchlists))
                watchlists.RemoveAll(w => w.Id == watchlistId);

            return Task.CompletedTask;
        }

        public Task AddSymbolToWatchlistAsync(string userId, string watchlistId, string symbol)
        {
            var watchlist = _userWatchlists[userId]?.Find(w => w.Id == watchlistId);
            watchlist?.Symbols.Add(symbol);
            return Task.CompletedTask;
        }

        public Task RemoveSymbolFromWatchlistAsync(string userId, string watchlistId, string symbol)
        {
            var watchlist = _userWatchlists[userId]?.Find(w => w.Id == watchlistId);
            watchlist?.Symbols.Remove(symbol);
            return Task.CompletedTask;
        }
    }

}