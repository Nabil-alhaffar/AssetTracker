using System;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Services
{
    public interface IPositionService
    {
        // Split a position based on the split factor
        Task SplitPositionAsync(Guid userId, string symbol, int splitFactor);

        // Check if a position has triggered the stop loss
        Task<bool> CheckPositionForStopLossAsync(Guid userId, string symbol, decimal stopLossPrice);

        // Update Profit & Loss for a given position
        Task UpdatePositionProfitLossAsync(Guid userId, string symbol);

        // Add or update a position in the portfolio
        Task AddOrUpdatePositionAsync(Guid userId, string symbol, decimal quantity, decimal price);

        // Reduce or remove a position from the portfolio
        Task ReduceOrRemovePositionAsync(Guid userId, string symbol, decimal quantity, decimal price);

        // Add or update a short position in the portfolio
        Task AddOrUpdateShortPositionAsync(Guid userId, string symbol, decimal quantity, decimal price);

        // Add position history
        Task AddPositionHistoryAsync(PositionHistory history);

        // Get PositionSummary for a user and symbol
        Task<PositionSummary> GetPositionSummaryAsync(Guid userId, string symbol);

        Task<List<PositionHistory>> GetPositionHistoryAsync(Guid userId, string symbol);

        Task<Position> GetPositionAsync(Guid userId, string symbol);


    }
}
