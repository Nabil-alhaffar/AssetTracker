using System;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly IWatchlistRepository _watchlistRepository;
        public readonly IUserRepository _userRepository;

        public WatchlistService(IWatchlistRepository repository, IUserRepository userRepository)
        {
            _watchlistRepository = repository;
            _userRepository = userRepository;
        }

        public async Task<List<Watchlist>> GetUserWatchlistsAsync(Guid userId) =>
            await _watchlistRepository.GetUserWatchlistsAsync(userId);

        public async Task AddWatchlistAsync(Guid userId, Watchlist watchlist)
        {
            // Check if the watchlist exceeds the max limit
            if (watchlist.Symbols.Count > 50)
                throw new Exception("A watchlist cannot have more than 50 stocks.");

            // Retrieve the user's existing watchlists
            var existingWatchlists = await _watchlistRepository.GetUserWatchlistsAsync(userId);

            // Check if a watchlist with the same name already exists
            if (existingWatchlists.Any(w => w.Name.Equals(watchlist.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("A watchlist with the same name already exists.");
            }

            // Retrieve the user object to add the watchlist to
            var user = await _userRepository.GetUserAsync(userId);

            if (user != null)
            {
                // Add the new watchlist to the user's watchlists
                user.Watchlists.Add(watchlist);

                // Update the user in the repository
                await _userRepository.UpdateUserAsync(user);
            }

            // Add the watchlist to the watchlist repository
            await _watchlistRepository.AddWatchlistAsync(userId, watchlist);
        }


        public async Task RemoveWatchlistAsync(Guid userId, Guid watchlistId)
        {
            await _watchlistRepository.RemoveWatchlistAsync(userId, watchlistId);
            var user = await _userRepository.GetUserAsync(userId); // Assuming this method exists
            if (user != null)
            {
                var watchlist = user.Watchlists.FirstOrDefault(w => w.Id == watchlistId);
                if (watchlist != null)
                {
                    user.Watchlists.Remove(watchlist);
                }
                await _userRepository.UpdateUserAsync(user);

            }
        }

        public async Task AddSymbolToWatchlistAsync(Guid userId, Guid watchlistId, string symbol)
        {
            var watchlists = await _watchlistRepository.GetUserWatchlistsAsync(userId);
            var watchlist = watchlists.Find(w => w.Id == watchlistId);

            if (watchlist == null)
                throw new Exception("Watchlist not found");

            if (watchlist.Symbols.Count >= 50)
                throw new Exception("A watchlist cannot have more than 50 stocks.");

            await _watchlistRepository.AddSymbolToWatchlistAsync(userId, watchlistId, symbol);
            var user = await _userRepository.GetUserAsync(userId); // Assuming this method exists
            if (user != null)
            {
                var _watchlist = user.Watchlists.FirstOrDefault(w => w.Id == watchlistId);
                if (_watchlist != null && !_watchlist.Symbols.Contains(symbol))
                {
                    _watchlist.Symbols.Add(symbol);  // Add symbol to the Watchlist
                }
                await _userRepository.UpdateUserAsync(user);

            }
        }

        public async Task RemoveSymbolFromWatchlistAsync(Guid userId, Guid watchlistId, string symbol)
        {
            await _watchlistRepository.RemoveSymbolFromWatchlistAsync(userId, watchlistId, symbol);
            var user = await _userRepository.GetUserAsync(userId); // Assuming this method exists
            if (user != null)
            {
                var watchlist = user.Watchlists.FirstOrDefault(w => w.Id == watchlistId);
                if (watchlist != null)
                {
                    watchlist.Symbols.Remove(symbol);  // Remove symbol from the Watchlist
                }
                await _userRepository.UpdateUserAsync(user);

            }
        }
    }

}

