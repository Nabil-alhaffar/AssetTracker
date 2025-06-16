using AssetTracker.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Repositories.Interfaces;

namespace AssetTracker.Repositories.MongoDBRepositories
{
    /// <summary>
    /// MongoDB implementation of the <see cref="IWatchlistRepository"/> interface.
    /// Provides methods to store and retrieve user watchlists stored in MongoDB.
    /// </summary>
    public class MongoWatchlistRepository : IWatchlistRepository
    {
        private readonly IMongoCollection<Watchlist> _watchlistCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoWatchlistRepository"/> class.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        public MongoWatchlistRepository(IMongoDatabase database)
        {
            _watchlistCollection = database.GetCollection<Watchlist>("Watchlists");
        }

        /// <summary>
        /// Retrieves all watchlists for the specified user.
        /// </summary>
        /// <param name="userId">The user ID to retrieve watchlists for.</param>
        /// <returns>A list of the user's watchlists.</returns>
        public async Task<List<Watchlist>> GetUserWatchlistsAsync(Guid userId)
        {
            var watchlists = await _watchlistCollection
                .Find(w => w.UserId == userId)
                .ToListAsync();

            return watchlists;
        }

        /// <summary>
        /// Adds a new watchlist for a user.
        /// </summary>
        /// <param name="userId">The user ID to associate with the watchlist.</param>
        /// <param name="watchlist">The watchlist to add.</param>
        public async Task AddWatchlistAsync(Guid userId, Watchlist watchlist)
        {
            watchlist.UserId = userId;
            await _watchlistCollection.InsertOneAsync(watchlist);
        }

        /// <summary>
        /// Removes a watchlist for a user by its ID.
        /// </summary>
        /// <param name="userId">The user ID associated with the watchlist.</param>
        /// <param name="watchlistId">The ID of the watchlist to remove.</param>
        public async Task RemoveWatchlistAsync(Guid userId, Guid watchlistId)
        {
            var result = await _watchlistCollection.DeleteOneAsync(w =>
                w.UserId == userId && w.WatchlistId == watchlistId);

            if (result.DeletedCount == 0)
            {
                throw new InvalidOperationException("Watchlist not found.");
            }
        }

        /// <summary>
        /// Adds a symbol to a user's watchlist.
        /// </summary>
        /// <param name="userId">The user ID associated with the watchlist.</param>
        /// <param name="watchlistId">The ID of the watchlist to update.</param>
        /// <param name="symbol">The stock symbol to add.</param>
        public async Task AddSymbolToWatchlistAsync(Guid userId, Guid watchlistId, string symbol)
        {
            var watchlist = await _watchlistCollection
                .Find(w => w.UserId == userId && w.WatchlistId == watchlistId)
                .FirstOrDefaultAsync();

            if (watchlist != null)
            {
                watchlist.Symbols.Add(symbol);
                var update = Builders<Watchlist>.Update.Set(w => w.Symbols, watchlist.Symbols);
                await _watchlistCollection.UpdateOneAsync(w => w.WatchlistId == watchlistId, update);
            }
        }

        /// <summary>
        /// Removes a symbol from a user's watchlist.
        /// </summary>
        /// <param name="userId">The user ID associated with the watchlist.</param>
        /// <param name="watchlistId">The ID of the watchlist to update.</param>
        /// <param name="symbol">The stock symbol to remove.</param>
        public async Task RemoveSymbolFromWatchlistAsync(Guid userId, Guid watchlistId, string symbol)
        {
            var watchlist = await _watchlistCollection
                .Find(w => w.UserId == userId && w.WatchlistId == watchlistId)
                .FirstOrDefaultAsync();

            if (watchlist != null)
            {
                watchlist.Symbols.Remove(symbol);
                var update = Builders<Watchlist>.Update.Set(w => w.Symbols, watchlist.Symbols);
                await _watchlistCollection.UpdateOneAsync(w => w.WatchlistId == watchlistId, update);
            }
        }
    }
}