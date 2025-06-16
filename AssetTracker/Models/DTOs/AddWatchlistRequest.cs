using System;
namespace AssetTracker.Models.DTOs
{
	public class AddWatchlistRequest
	{
        public string WatchlistName { get; set; } = null!;
        public string[]? Symbols { get; set; }
    }
}

