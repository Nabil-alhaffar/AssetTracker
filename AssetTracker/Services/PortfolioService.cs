using System;
using AssetTracker.Models;
namespace AssetTracker.Services
{
	public class PortfolioService:IPortfolioService
	{
        private readonly IPositionService _positionService;
        public PortfolioService(IPositionService positionService)
		{
			_positionService = positionService;
		}
		public async Task<double> GetTotalValueAsync() {
			var positions = await _positionService.GetAllPositionsAsync();
			return positions.Sum(p => p.GetMarketValue());
		}
		public async Task <double> GetTotalProfitAndLossAsync()
		{
			var positions = await _positionService.GetAllPositionsAsync();
			return positions.Sum(p => p.GetProfitLoss());
		}
		public async Task AddPositionToPortfolioAsync(Position position)
		{
			await _positionService.AddPositionAsync(position);

			//var currentStockprice = await _stockService.GetStockPriceAsync(symbol);
			//var stock = new Stock(symbol, quantity);
		}
		public async Task RemovePositionAsync(string symbol)
		{
			await _positionService.RemovePositionAsync(symbol);

		}
	}
}

