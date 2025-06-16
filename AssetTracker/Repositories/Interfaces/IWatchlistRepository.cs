using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for managing user watchlists and their symbols.
    /// </summary>
    public interface IWatchlistRepository
    {
        /// <summary>
        /// Retrieves all watchlists for a given user asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A list of watchlists belonging to the user.</returns>
        Task<List<Watchlist>> GetUserWatchlistsAsync(Guid userId);

        /// <summary>
        /// Adds a new watchlist for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="watchlist">The watchlist to add.</param>
        Task AddWatchlistAsync(Guid userId, Watchlist watchlist);

        /// <summary>
        /// Removes a watchlist by its identifier for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="watchlistId">The watchlist identifier to remove.</param>
        Task RemoveWatchlistAsync(Guid userId, Guid watchlistId);

        /// <summary>
        /// Adds a stock symbol to a specific watchlist asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="watchlistId">The watchlist identifier.</param>
        /// <param name="symbol">The stock symbol to add.</param>
        Task AddSymbolToWatchlistAsync(Guid userId, Guid watchlistId, string symbol);

        /// <summary>
        /// Removes a stock symbol from a specific watchlist asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="watchlistId">The watchlist identifier.</param>
        /// <param name="symbol">The stock symbol to remove.</param>
        Task RemoveSymbolFromWatchlistAsync(Guid userId, Guid watchlistId, string symbol);
    }
}