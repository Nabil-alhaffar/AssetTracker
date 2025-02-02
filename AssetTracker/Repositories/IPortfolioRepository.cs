using System;
using AssetTracker.Models;

namespace AssetTracker.Repositories
{
    public interface IPortfolioRepository
    {
        Task<Portfolio> GetUserPortfolioAsync(int userId);
        Task AddPortfolioAsync(Portfolio portfolio);
        //Task<IEnumerable<Portfolio>> GetUsersWatchLists(int userId);

        //Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync();
        //Task UpdatePortfolioAsync(Portfolio portfolio);
        //Task RemovePortfolioAsync(int portfolioId);
        //Task AddPositionToPortfolioAsync(int portfolioId, Position position);
        //Task RemovePositionFromPortfolioAsync(int portfolioId, string symbol);



    }
}

