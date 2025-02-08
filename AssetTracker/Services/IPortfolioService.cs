using System;
using AssetTracker.Models;

	

    public interface IPortfolioService
    {
        Task<double> GetTotalValueAsync(int userId);
        Task<double> GetTotalProfitAndLossAsync(int userId);
        Task<ICollection<Position>> GetAllPositionsAsync(int userId);
        Task AddPositionToPortfolioAsync(Position position, int userId);
        Task RemovePositionAsync(int userId,string stockSymbol);
        Task<Portfolio> GetPortfolioAsync(int userId);
        Task<PortfolioSummary> GetPortfolioSummaryAsync(int userId);


    }




