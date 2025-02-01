using System;
using AssetTracker.Models;

namespace AssetTracker.Services
{
    public class PositionService : IPositionService
    {
        private readonly StockService _stockService;
        private static readonly List<Position> _positions = new List<Position>();

        public  PositionService()
        {
            //_positions = new List<Position>();
        }

        // Create a new position by fetching stock data from Alpha Vantage

        public async Task AddPositionAsync(Position position)
        {
            _positions.Add(position);
            await Task.CompletedTask;
        }
        public async Task RemovePositionAsync(string stockSymbol)
        {
            var position = _positions.FirstOrDefault(p => p.StockSymbol == stockSymbol);
            if (position != null)
            {
                _positions.Remove(position);
            }
            await Task.CompletedTask;
        }

        public async Task<Position> GetPositionAsync(string stockSymbol)
        {
            var position = _positions.FirstOrDefault(p => p.StockSymbol == stockSymbol);
            return await Task.FromResult(position);
        }
        public async Task<List<Position>> GetAllPositionsAsync()
        {
            return await Task.FromResult(_positions);
        }
    }
}

