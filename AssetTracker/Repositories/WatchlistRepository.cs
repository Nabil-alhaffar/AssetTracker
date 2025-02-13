using System.Collections.Generic;
using AssetTracker.Models;
using AssetTracker.Repositories;
namespace AssetTracker.Repositories
{
    public class WatchlistRepository : IWatchlistRepository
    {
        private readonly Dictionary<Guid, List<Watchlist>> _userWatchlists ;

        public WatchlistRepository()
        {
            _userWatchlists = new ();
            //_context = context;
        }   

        public Task<List<Watchlist>> GetUserWatchlistsAsync(Guid userId) =>
            Task.FromResult(_userWatchlists.TryGetValue(userId, out var watchlists) ? watchlists : new List<Watchlist>());

        public Task AddWatchlistAsync(Guid userId, Watchlist watchlist)
        {
            if (!_userWatchlists.ContainsKey(userId))
                _userWatchlists[userId] = new List<Watchlist>();

            _userWatchlists[userId].Add(watchlist);
            return Task.CompletedTask; // No actual async operation here, but keeps API consistent
        }

        public Task RemoveWatchlistAsync(Guid userId, Guid watchlistId)
        {
            if (_userWatchlists.TryGetValue(userId, out var watchlists))
                watchlists.RemoveAll(w => w.Id == watchlistId);

            return Task.CompletedTask;
        }

        public Task AddSymbolToWatchlistAsync(Guid userId, Guid watchlistId, string symbol)
        {
            var watchlist = _userWatchlists[userId]?.Find(w => w.Id == watchlistId);
            watchlist?.Symbols.Add(symbol);
            return Task.CompletedTask;
        }

        public Task RemoveSymbolFromWatchlistAsync(Guid userId, Guid watchlistId, string symbol)
        {
            var watchlist = _userWatchlists[userId]?.Find(w => w.Id == watchlistId);
            watchlist?.Symbols.Remove(symbol);
            return Task.CompletedTask;
        }
    }

}