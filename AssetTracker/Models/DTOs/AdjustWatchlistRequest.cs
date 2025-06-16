using System;
namespace AssetTracker.Models.DTOs
{
    /// <summary>
    /// Represents a request to adjust an existing watchlist with a list of stock symbols to be added or removed.
    /// </summary>
    public sealed record AdjustWatchlistRequest
	{

        /// <summary>
        /// Array of stock symbols to be added or removed from the watchlist.
        /// </summary>
        public string[] Symbols { get; set; } = null!;

    }
}

