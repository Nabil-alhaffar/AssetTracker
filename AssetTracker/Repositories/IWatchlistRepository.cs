using AssetTracker.Models;
namespace AssetTracker.Repositories
{
    public interface IWatchlistRepository
    {
        Task<List<Watchlist>> GetUserWatchlistsAsync(string userId);
        Task AddWatchlistAsync(string userId, Watchlist watchlist);
        Task RemoveWatchlistAsync(string userId, string watchlistId);
        Task AddSymbolToWatchlistAsync(string userId, string watchlistId, string symbol);
        Task RemoveSymbolFromWatchlistAsync(string userId, string watchlistId, string symbol);
    }

}