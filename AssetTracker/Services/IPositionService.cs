using System;
using AssetTracker.Models;

namespace AssetTracker.Services
{
	public interface IPositionService
	{
        Task AddPositionAsync(Position position, int userId);
        //Task RemovePositionAsync(string stockSymbol);
        //Task<Position> GetPositionAsync(string stockSymbol);
        //Task<List<Position>> GetAllPositionsAsync();
        public Task UpdatePositionAsync(Position position, int userId);
        public Task SplitPositionAsync(int userId, string symbol, int splitFactor);
        public Task <bool> CheckPositionForStopLossAsync(int userId, string symbol, decimal stopLossPrice);
        public Task UpdatePositionProfitLossAsync(int userId, string symbol);
        Task<List<PositionHistory>> GetPositionHistoryAsync(int userId, string symbol);
        Task AddPositionHistoryAsync(PositionHistory history);



    }
}

