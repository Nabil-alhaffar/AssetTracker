using System;
using AssetTracker.Models;

	

    public interface IPortfolioService
    {
        Task<double> GetTotalValueAsync();
        Task<double> GetTotalProfitAndLossAsync();
        Task AddPositionToPortfolioAsync(Position position);
        Task RemovePositionAsync(string stockSymbol);
    }




