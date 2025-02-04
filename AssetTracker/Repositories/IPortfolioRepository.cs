using System;
using AssetTracker.Models;

namespace AssetTracker.Repositories
{
    public interface IPortfolioRepository
    {
        Task<Portfolio> GetUserPortfolioAsync(int userId);
        Task AddPortfolioAsync(Portfolio portfolio);
        public Task AddPositionToPortfolioAsync( Position position, int portfolioId);
        public Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync();
        public Task RemovePositionFromPortfolioAsync(int portfolioId, string symbol);
        public Task RemovePortfolioAsync(int userId);
        public Task UpdatePortfolioAsync(Portfolio portfolio);
        public Task<ICollection<Position>> GetPositionsByUserId(int userId);
        //Task<IEnumerable<Portfolio>> GetUsersWatchLists(int userId);

        //Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync();
        //Task UpdatePortfolioAsync(Portfolio portfolio);
        //Task RemovePortfolioAsync(int portfolioId);
        //Task AddPositionToPortfolioAsync(int portfolioId, Position position);
        //Task RemovePositionFromPortfolioAsync(int portfolioId, string symbol);



    }
}

