using System;
using AssetTracker.Models;

namespace AssetTracker.Services
{
	public interface IPositionService
	{
        Task AddPositionAsync(Position position);
        Task RemovePositionAsync(string stockSymbol);
        Task<Position> GetPositionAsync(string stockSymbol);
        Task<List<Position>> GetAllPositionsAsync();
    }
}

