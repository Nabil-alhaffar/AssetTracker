using System;
using AssetTracker.Models;
using AssetTracker.Repositories;
using AssetTracker.Repositories.Interfaces;
using AssetTracker.Repositories.MongoDBRepositories;
using AssetTracker.Services.Interfaces;

namespace AssetTracker.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly IWatchlistRepository _watchlistRepository;
        public readonly IUserRepository _userRepository;

        public WatchlistService(AppSettings settings, IWatchlistRepository watchlistRepository, IUserRepository userRepository)
        {
            _watchlistRepository = watchlistRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Watchlist>> GetUserWatchlistsAsync(Guid userId) =>
            await _watchlistRepository.GetUserWatchlistsAsync(userId);
         
        public async Task AddWatchlistAsync(Guid userId, string watchlistName, string[]? symbols = null)
        {
            // Check if the watchlist exceeds the max limit
            //if (watchlist.Symbols.Count > 50)
            //    throw new Exception("A watchlist cannot have more than 50 stocks.");


            if (string.IsNullOrWhiteSpace(watchlistName))
                throw new ArgumentException("Watchlist name cannot be empty.");

            // Retrieve the user's existing watchlists
            var existingWatchlists = await _watchlistRepository.GetUserWatchlistsAsync(userId);

            // Check if a watchlist with the same name already exists
            if (existingWatchlists.Any(w => w.Name.Equals(watchlistName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("A watchlist with the same name already exists.");
            }


            var watchlist = new Watchlist{ Name = watchlistName, UserId = userId };

            await _watchlistRepository.AddWatchlistAsync(userId, watchlist);

            if (symbols != null)
            {
                var watchlistId = await GetUserWatchlistIdByName(userId, watchlistName);

                await AddSymbolsToWatchlistAsync(userId, (Guid)watchlistId, symbols);
                
            }
        }


        public async Task RemoveWatchlistAsync(Guid userId, Guid watchlistId)
        {
            try
            {
                await _watchlistRepository.RemoveWatchlistAsync(userId, watchlistId);

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to remove watchlist: ", ex);
            }
            //var user = await _userRepository.GetUserByIDAsync(userId); // Assuming this method exists
            //if (user != null)
            //{
            //    var watchlist = user.Watchlists.FirstOrDefault(w => w.WatchlistId == watchlistId);
            //    if (watchlist != null)
            //    {
            //        user.Watchlists.Remove(watchlist);
            //    }
            //    await _userRepository.UpdateUserAsync(user);

            //}
        }

        public async Task AddSymbolsToWatchlistAsync(Guid userId, Guid watchlistId, IEnumerable <string> symbols)
        {
            try
            {
                var watchlists = await _watchlistRepository.GetUserWatchlistsAsync(userId);
                var watchlist = watchlists.Find(w => w.WatchlistId == watchlistId);

                if (watchlist == null)
                    throw new Exception("Watchlist not found");

                var existingCount = watchlist.Symbols?.Count ?? 0;
                var incomingCount = symbols.Count();

                if (existingCount + incomingCount > 50)
                    throw new Exception("Adding these symbols would exceed the 50-stock limit.");

                foreach (var symbol in symbols.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    if (!watchlist.Symbols.Contains(symbol, StringComparer.OrdinalIgnoreCase))
                    {
                        await _watchlistRepository.AddSymbolToWatchlistAsync(userId, watchlistId, symbol);
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Unable to add symbol to watchlist", ex);
            }
            //var user = await _userRepository.GetUserByIDAsync(userId); // Assuming this method exists
            //if (user != null)
            //{
            //    var _watchlist = user.Watchlists.FirstOrDefault(w => w.WatchlistId == watchlistId);
            //    if (_watchlist != null && !_watchlist.Symbols.Contains(symbol))
            //    {
            //        _watchlist.Symbols.Add(symbol);  // Add symbol to the Watchlist
            //    }
            //    await _userRepository.UpdateUserAsync(user);

            //}
        }



        public async Task RemoveSymbolsFromWatchlistAsync(Guid userId, Guid watchlistId, IEnumerable<string> symbols)
        {
            try
            {
                var watchlists = await _watchlistRepository.GetUserWatchlistsAsync(userId);
                var watchlist = watchlists.Find(w => w.WatchlistId == watchlistId);

                if (watchlist == null)
                    throw new Exception("Watchlist not found");

                foreach (var symbol in symbols.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    if (watchlist.Symbols.Contains(symbol, StringComparer.OrdinalIgnoreCase))
                    {
                        await _watchlistRepository.RemoveSymbolFromWatchlistAsync(userId, watchlistId, symbol);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to remove symbol from watchlist", ex);
            }
        }

        public async Task <List<string>> GetAllWatchedTickersByUserIdAsync (Guid userId)
        {
           List<string> tickers = new(); 
           var watchlists = await _watchlistRepository.GetUserWatchlistsAsync(userId);
           foreach (var watchlist in watchlists)
            {
                tickers.AddRange(watchlist.Symbols);
            }
            return tickers.Distinct().ToList();

        }

        public async Task<Guid?> GetUserWatchlistIdByName(Guid userId, string watchlistName)
        {
            var watchlists = await _watchlistRepository.GetUserWatchlistsAsync(userId);

            var match = watchlists.FirstOrDefault(w => w.Name.Equals(watchlistName, StringComparison.OrdinalIgnoreCase));
            if (match == null)
                throw new KeyNotFoundException("Watchlist not found");
            return match?.WatchlistId;
        }
    }

}

