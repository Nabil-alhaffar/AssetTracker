using System;
using AssetTracker.Models;


namespace AssetTracker.Services.Interfaces
{

    public interface IPortfolioService
    {
        //Task<decimal> GetTotalValueAsync(Guid userId);
        //Task<decimal> GetTotalProfitAndLossAsync(Guid userId);

        Task UpdateAvailableFundsAsync(Guid userId, decimal additionalAmount);
        Task<decimal> GetAvailableFundsAsync(Guid userId);
        Task<PortfolioSummary> GetPortfolioSummaryAsync(Guid userId);
        Task<PortfolioPerformance> GetPortfolioPerformanceAsync(Guid userId, int days);
        Task<Dictionary<string, Position>> GetPortfolioPositionsAsync(Guid userId);
        public Task<Position> GetUserPositionBySymbol(Guid userId, string symbol);
        Task<Portfolio> GetUserPortfolioAsync(Guid userId);
        public Task UpdateTotalValuesForAllUsersAsync();
        public Task UpdatePortfolioForAllUsersAsync();
        public Task UpdatePortfolioByUserId(Guid userId);

        //Task<ICollection<Position>> GetAllPositionsAsync(Guid userId);
        //Task AddPositionToPortfolioAsync(Position position, Guid userId);
        //Task RemovePositionAsync(Guid userId,string stockSymbol);
        //Task<Portfolio> GetPortfolioAsync(Guid userId);


    }




}