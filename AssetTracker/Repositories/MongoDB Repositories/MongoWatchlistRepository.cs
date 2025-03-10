using AssetTracker.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Repositories.Interfaces;

namespace AssetTracker.Repositories.MongoDBRepositories
{
    public class MongoWatchlistRepository : IWatchlistRepository
    {
        private readonly IMongoCollection<Watchlist> _watchlistCollection;

        // Constructor to initialize the MongoDB collection
        public MongoWatchlistRepository(IMongoDatabase database)
        {
            _watchlistCollection = database.GetCollection<Watchlist>("Watchlists");  // Directly using Watchlist model
        }

        // Get all watchlists for a specific user
        public async Task<List<Watchlist>> GetUserWatchlistsAsync(Guid userId)
        {
            var watchlists = await _watchlistCollection
                .Find(w => w.UserId == userId)  // Filter by userId
                .ToListAsync();

            return watchlists;
        }

        // Add a new watchlist for a user
        public async Task AddWatchlistAsync(Guid userId, Watchlist watchlist)
        {
            watchlist.UserId = userId;  // Associate userId with the watchlist

            await _watchlistCollection.InsertOneAsync(watchlist);  // Insert the watchlist directly
        }

        // Remove a specific watchlist by its ID for a user
        public async Task RemoveWatchlistAsync(Guid userId, Guid watchlistId)
        {
            var result = await _watchlistCollection.DeleteOneAsync(w =>
                w.UserId == userId && w.WatchlistId == watchlistId);

            if (result.DeletedCount == 0)
            {
                throw new InvalidOperationException("Watchlist not found.");
            }
        }

        // Add a symbol to an existing watchlist
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

        // Remove a symbol from an existing watchlist
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
