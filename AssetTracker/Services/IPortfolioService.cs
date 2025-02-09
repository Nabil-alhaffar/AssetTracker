using System;
using AssetTracker.Models;

	

    public interface IPortfolioService
    {
        Task<double> GetTotalValueAsync(Guid userId);
        Task<double> GetTotalProfitAndLossAsync(Guid userId);
        Task<ICollection<Position>> GetAllPositionsAsync(Guid userId);
        Task AddPositionToPortfolioAsync(Position position, Guid userId);
        Task RemovePositionAsync(Guid userId,string stockSymbol);
        Task<Portfolio> GetPortfolioAsync(Guid userId);
        Task<PortfolioSummary> GetPortfolioSummaryAsync(Guid userId);


    }




