using System;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services
{
	public class PortfolioService:IPortfolioService
	{
        private static readonly IPositionRepository _positionRepository = new PositionRepository();
        //private readonly IPositionService _positionService;
		//private readonly IP
        public PortfolioService(IPositionRepository positionRepository)
		{
			//_positionRepository = positionRepository;
		}
		public async Task<double> GetTotalValueAsync() {
			var positions = await _positionRepository.GetAllPositionsAsync();
			return positions.Sum(p => p.GetMarketValue());
		}

        public async Task<List<Position>> GetAllPositionsAsync()
        {
            var positions = await _positionRepository.GetAllPositionsAsync();
            return (List<Position>)positions;
        }
		public async Task <double> GetTotalProfitAndLossAsync()
		{
			var positions = await _positionRepository.GetAllPositionsAsync();
			return positions.Sum(p => p.GetProfitLoss());
		}
		public async Task  AddPositionToPortfolioAsync(Position position)
		{
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position), "Position cannot be null");
            }

            // You might want to add additional checks, e.g., if the position already exists
            await _positionRepository.AddPositionAsync(position);

            //var currentStockprice = await _stockService.GetStockPriceAsync(symbol);
            //var stock = new Stock(symbol, quantity);
        }
        public async Task RemovePositionAsync(string symbol)
		{
            var position = await _positionRepository.GetPositionBySymbolAsync(symbol);
            if (position == null)
            {
                throw new Exception("Position not found");
            }

            await _positionRepository.RemovePositionAsync(symbol);
        }

	}
}

