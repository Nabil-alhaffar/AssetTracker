using System;
using AssetTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetTracker.Repositories
{
	public interface IPositionRepository
	{
        Task<IEnumerable<Position>> GetAllPositionsAsync();
        Task<Position> GetPositionBySymbolAsync(string symbol);
        Task AddPositionAsync(Position position);
        Task RemovePositionAsync(string symbol);
    }
}

