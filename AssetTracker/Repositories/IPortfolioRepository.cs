using System;
using AssetTracker.Models;

namespace AssetTracker.Repositories
{
    public interface IPortfolioRepository
    {
        Task<Portfolio> GetUserPortfolioAsync(Guid userId);
        Task AddPortfolioAsync(Portfolio portfolio);
        public Task AddPositionToPortfolioAsync( Position position, Guid portfolioId);
        public Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync();
        public Task RemovePositionFromPortfolioAsync(Guid portfolioId, string symbol);
        public Task RemovePortfolioAsync(Guid userId);
        public Task UpdatePortfolioAsync(Portfolio portfolio);
        public Task<ICollection<Position>> GetPositionsByUserId(Guid userId);
        //Task<IEnumerable<Portfolio>> GetUsersWatchLists(int userId);

        //Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync();
        //Task UpdatePortfolioAsync(Portfolio portfolio);
        //Task RemovePortfolioAsync(int portfolioId);
        //Task AddPositionToPortfolioAsync(int portfolioId, Position position);
        //Task RemovePositionFromPortfolioAsync(int portfolioId, string symbol);



    }
}

