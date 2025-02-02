//using AssetTracker.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;namespace AssetTracker.Repositories
//{
//    public class PositionRepository : IPositionRepository
//    {
//        private readonly List<Position> _positions; // Simulating an in-memory store (replace with actual DB logic)

//        public PositionRepository()
//        {
//            _positions = new List<Position>(); // Initialize with an empty list

//        }

//        public async Task AddPositionAsync(Position position)
//        {
//            _positions.Add(position); // Add position to the in-memory list (replace with DB save logic)
//            await Task.CompletedTask; // Simulate async task
//        }

//        public async Task<IEnumerable<Position>> GetAllPositionsAsync()
//        {
//            return await Task.FromResult(_positions); // Simulate async behavior
//        }

//        public async Task<Position> GetPositionBySymbolAsync(string symbol)
//        {
//            var position = _positions.FirstOrDefault(p => p.StockSymbol == symbol);
//            return await Task.FromResult(position); // Simulate async behavio        
//        }
//        public async Task RemovePositionAsync(string symbol)
//        {
//            var position = _positions.FirstOrDefault(p => p.StockSymbol == symbol);
//            if (position != null)
//            {
//                _positions.Remove(position); // Remove position from the in-memory list (replace with DB delete logic)
//            }
//            await Task.CompletedTask; // S
//        }
//    }
//}


