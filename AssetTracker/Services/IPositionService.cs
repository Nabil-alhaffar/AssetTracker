using System;
using AssetTracker.Models;

namespace AssetTracker.Services
{
	public interface IPositionService
	{
        Task AddPositionAsync(Position position, Guid userId);
        //Task RemovePositionAsync(string stockSymbol);
        //Task<Position> GetPositionAsync(string stockSymbol);
        //Task<List<Position>> GetAllPositionsAsync();
        public Task UpdatePositionAsync(Position position, Guid userId);
        public Task SplitPositionAsync(Guid userId, string symbol, int splitFactor);
        public Task <bool> CheckPositionForStopLossAsync(Guid userId, string symbol, decimal stopLossPrice);
        public Task UpdatePositionProfitLossAsync(Guid userId, string symbol);
        Task<List<PositionHistory>> GetPositionHistoryAsync(Guid userId, string symbol);
        Task AddPositionHistoryAsync(PositionHistory history);



    }
}

