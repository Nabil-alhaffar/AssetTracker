using System;
using AssetTracker.Models;

	

    public interface IPortfolioService
    {
        //Task<decimal> GetTotalValueAsync(Guid userId);
        //Task<decimal> GetTotalProfitAndLossAsync(Guid userId);

        Task UpdateAvailableFundsAsync(Guid userId, decimal additionalAmount);
        Task<decimal> GetAvailableFundsAsync(Guid userId);
        public Task<PortfolioSummary> GetPortfolioSummaryAsync(Guid userId);
        public Task<PortfolioPerformance> GetPortfolioPerformanceAsync(Guid userId, int days);
        //Task<ICollection<Position>> GetAllPositionsAsync(Guid userId);
        //Task AddPositionToPortfolioAsync(Position position, Guid userId);
        //Task RemovePositionAsync(Guid userId,string stockSymbol);
        //Task<Portfolio> GetPortfolioAsync(Guid userId);


    }




