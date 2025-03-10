using System;
using AssetTracker.Models;

namespace AssetTracker.Repositories.Interfaces
{
    public interface IPortfolioRepository
    {
        public Task<Portfolio> GetUserPortfolioAsync(Guid userId);
        public Task UpdatePortfolioAsync(Portfolio portfolio);
        public Task<Dictionary<string, Position>> GetPositionsByUserId(Guid userId);
        public Task<Position> GetUserPositionBySymbol(Guid userId, string symbol);
        public Task AddPortfolioAsync(Portfolio portfolio);
        //public Task StoreMarketValueAsync(Guid userId, DateOnly date, decimal marketValue);
        //public Task<decimal?> GetMarketValueOnDateAsync(Guid userId, DateOnly date);

        //public DateTime? GetEarliestMarketValueDate(Guid userId);

        //Task AddPortfolioAsync(Portfolio portfolio);
        //public Task AddPositionToPortfolioAsync( Position position, Guid portfolioId);
        //public Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync();
        //public Task RemovePositionFromPortfolioAsync(Guid portfolioId, string symbol);
        //public Task RemovePortfolioAsync(Guid userId);
        ////Task<IEnumerable<Portfolio>> GetUsersWatchLists(int userId);

        //Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync();
        //Task UpdatePortfolioAsync(Portfolio portfolio);
        //Task RemovePortfolioAsync(int portfolioId);
        //Task AddPositionToPortfolioAsync(int portfolioId, Position position);
        //Task RemovePositionFromPortfolioAsync(int portfolioId, string symbol);



    }
}

