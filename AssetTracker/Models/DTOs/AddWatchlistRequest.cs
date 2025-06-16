using System;
namespace AssetTracker.Models.DTOs
{
    /// <summary>
    /// Represents a request to create a new watchlist with an optional list of stock symbols.
    /// </summary>
	public sealed record AddWatchlistRequest
	{
        /// <summary>
        /// The name of the watchlist to be created.
        /// </summary>
        public string WatchlistName { get; set; } = null!;

        /// <summary>
        /// Optional array of stock symbols to include in the watchlist.
        /// </summary>
        public string[]? Symbols { get; set; }
    }
}

