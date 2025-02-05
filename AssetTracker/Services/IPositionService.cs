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
        public Task UpdatePositionAsync(int userId, string symbol, double additionalQuantity, double purchasePrice);
        public Task SplitPositionAsync(int userId, string symbol, int splitFactor);
        public Task <bool> CheckPositionForStopLossAsync(int userId, string symbol, double stopLossPrice);
        public Task UpdatePositionProfitLossAsync(int userId, string symbol);




    }
}

