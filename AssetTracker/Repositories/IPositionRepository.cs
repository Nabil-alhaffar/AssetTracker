using System;
using AssetTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

//namespace AssetTracker.Repositories
//{
//	public interface IPositionRepository
//	{
//        Task<IEnumerable<Position>> GetAllPositionsAsync();
//        Task<Position> GetPositionBySymbolAsync(string symbol);
//        Task AddPositionAsync(Position position);
//        Task RemovePositionAsync(string symbol);
//    }
//}

using AssetTracker.Models;

public interface IPositionRepository
{
    Task<IEnumerable<Position>> GetAllPositions();
    Task<Position> GetPositionAsync(Guid userId, string symbol);  // Get position for the user
    Task UpdatePositionAsync(Position position);  // Update position details
    Task AddPositionAsync(Position position);  // Add new position
    Task<bool> DeletePositionAsync(Guid userId, string symbol);  // Delete position
}
