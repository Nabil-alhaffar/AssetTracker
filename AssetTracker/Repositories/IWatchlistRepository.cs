using AssetTracker.Models;
namespace AssetTracker.Repositories
{
    public interface IWatchlistRepository
    {
        Task<List<Watchlist>> GetUserWatchlistsAsync(Guid userId);
        Task AddWatchlistAsync(Guid userId, Watchlist watchlist);
        Task RemoveWatchlistAsync(Guid userId, Guid watchlistId);
        Task AddSymbolToWatchlistAsync(Guid userId, Guid watchlistId, string symbol);
        Task RemoveSymbolFromWatchlistAsync(Guid userId, Guid watchlistId, string symbol);
    }

}