//using AssetTracker.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;namespace AssetTracker.Repositories


using AssetTracker.Models;

public class PositionRepository : IPositionRepository
{
    private static List<Position> _positions = new List<Position>();

    //public PositionRepository (IPositionRepository positionRepository)
    //{
    //    _positions = ;
    //}

    public Task<Position> GetPositionAsync(int userId, string symbol)
    {
        var position = _positions.FirstOrDefault(p => p.UserId == userId && p.StockSymbol == symbol);
        return Task.FromResult(position);
    }
    public async Task<IEnumerable<Position>> GetAllPositions()
    {
         return await Task.FromResult(_positions);
    }

    public Task UpdatePositionAsync(Position position)
    {
        var existingPosition = _positions.FirstOrDefault(p => p.UserId == position.UserId && p.StockSymbol == position.StockSymbol);
        if (existingPosition != null)
        {
            existingPosition.Quantity = position.Quantity;
            existingPosition.AveragePurchasePrice = position.AveragePurchasePrice;
            // Update other properties as needed
        }
        return Task.CompletedTask;
    }

    public Task AddPositionAsync(Position position)
    {
        _positions.Add(position);
        return Task.CompletedTask;
    }

    public Task<bool> DeletePositionAsync(int userId, string symbol)
    {
        var position = _positions.FirstOrDefault(p => p.UserId == userId && p.StockSymbol == symbol);
        if (position != null)
        {
            _positions.Remove(position);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
