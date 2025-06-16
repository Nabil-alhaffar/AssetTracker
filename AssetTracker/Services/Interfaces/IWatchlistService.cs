using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Provides methods to manage user watchlists and their symbols.
    /// </summary>
    public interface IWatchlistService
    {
        /// <summary>
        /// Gets all watchlists belonging to a specific user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A list of watchlists for the user.</returns>
        Task<List<Watchlist>> GetUserWatchlistsAsync(Guid userId);

        /// <summary>
        /// Adds a new watchlist for a user with optional initial symbols.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="watchlistName">The name of the new watchlist.</param>
        /// <param name="symbols">Optional list of stock symbols to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddWatchlistAsync(Guid userId, string watchlistName, string[]? symbols = null);

        /// <summary>
        /// Removes a watchlist by its ID for a given user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="watchlistId">The watchlist's unique identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveWatchlistAsync(Guid userId, Guid watchlistId);

        /// <summary>
        /// Adds multiple stock symbols to an existing watchlist.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="watchlistId">The watchlist's unique identifier.</param>
        /// <param name="symbols">The symbols to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddSymbolsToWatchlistAsync(Guid userId, Guid watchlistId, IEnumerable<string> symbols);

        /// <summary>
        /// Removes multiple stock symbols from an existing watchlist.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="watchlistId">The watchlist's unique identifier.</param>
        /// <param name="symbol">The symbols to remove.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveSymbolsFromWatchlistAsync(Guid userId, Guid watchlistId, IEnumerable<string> symbol);

        /// <summary>
        /// Retrieves all stock symbols watched by the user across all watchlists.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A list of all watched ticker symbols.</returns>
        Task<List<string>> GetAllWatchedTickersByUserIdAsync(Guid userId);

        /// <summary>
        /// Gets the ID of a watchlist by its name for a given user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="watchlistName">The name of the watchlist.</param>
        /// <returns>The watchlist ID if found; otherwise, null.</returns>
        Task<Guid?> GetUserWatchlistIdByName(Guid userId, string watchlistName);
    }
}