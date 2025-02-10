////using AssetTracker.Models;
////using System.Collections.Generic;
////using System.Linq;
////using System.Threading.Tasks;namespace AssetTracker.Repositories


//using AssetTracker.Models;
//using AssetTracker.Repositories; 

//public class PositionRepository : IPositionRepository
//{

//    private readonly IPortfolioRepository _portfolioRepository;
//    public PositionRepository(IPortfolioRepository portfolioRepository)

//    {
//        _portfolioRepository = portfolioRepository ;
//    }

//    public async Task<Position?> GetPositionAsync(Guid userId, string symbol)
//    {
//        var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
//        return portfolio.Positions.TryGetValue(symbol, out var position) ? position:null;
//    }

//    public async Task AddOrUpdatePositionAsync(Guid userId, Position position)
//    {
//        var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
//        if (portfolio == null) return;
//        portfolio.Positions[position.Symbol] = position;
//        await _portfolioRepository.UpdatePortfolioAsync(portfolio);

//    }

//    public async Task<IEnumerable<Position>> GetAllPositions()
//    {
//         return await Task.FromResult(_positions);
//    }

//    public Task UpdatePositionAsync(Position position)
//    {
//        var existingPosition = _positions.FirstOrDefault(p => p.UserId == position.UserId && p.StockSymbol == position.StockSymbol);
//        if (existingPosition != null)
//        {
//            existingPosition.Quantity = position.Quantity;
//            existingPosition.AveragePurchasePrice = position.AveragePurchasePrice;
//            // Update other properties as needed
//        }
//        return Task.CompletedTask;
//    }

//    public Task AddPositionAsync(Position position)
//    {
//        _positions.Add(position);
//        return Task.CompletedTask;
//    }

//    public Task<bool> DeletePositionAsync(Guid userId, string symbol)
//    {
//        var position = _positions.FirstOrDefault(p => p.UserId == userId && p.StockSymbol == symbol);
//        if (position != null)
//        {
//            _positions.Remove(position);
//            return Task.FromResult(true);
//        }
//        return Task.FromResult(false);
//    }
//}
