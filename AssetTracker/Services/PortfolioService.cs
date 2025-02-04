using System;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services
{
	public class PortfolioService:IPortfolioService
	{
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IStockService _stockService;  // To get the market price of the stock

        //private readonly IPositionService _positionService;
        //private readonly IP
        public PortfolioService(IPortfolioRepository portfolioRepository, IStockService stockService)
        {
            _portfolioRepository = portfolioRepository;
            _stockService = stockService;
        }
        public async Task<double> GetTotalValueAsync(int userId) {
            try
            {
                var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
                return (double)portfolio.Positions.Sum(p => p.GetMarketValue());

            }
            catch
            {
                throw new ArgumentNullException(nameof(userId), "User cannot be null.");
            }
		}

        public async Task<ICollection<Position>> GetAllPositionsAsync(int userId)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var positions = portfolio.Positions;
            return positions;
        }
		public async Task <double> GetTotalProfitAndLossAsync(int userId)
		{
            try
            {
                var positions = await _portfolioRepository.GetPositionsByUserId(userId);
                return (double)positions.Sum(p => p.GetProfitLoss());
            }
            catch
            {
                throw new ArgumentException(nameof(userId), "No positions for provided userID.");
            }
		}
		public async Task  AddPositionToPortfolioAsync(Position position,int userId)
		{
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position), "Position cannot be null.");
            }

            // You might want to add additional checks, e.g., if the position already exists
            await _portfolioRepository.AddPositionToPortfolioAsync( position, userId);

            //var currentStockprice = await _stockService.GetStockPriceAsync(symbol);
            //var stock = new Stock(symbol, quantity);
        }
        public async Task RemovePositionAsync(int userId, string symbol) 
		{
            try
            {
                await _portfolioRepository.RemovePositionFromPortfolioAsync(userId, symbol);
            }
            catch
            {
                throw new Exception("Position not found");
            }
            //if (position == null)
            //{
            //}

            //await _positionRepository.RemovePositionAsync(symbol);
        }
        public async Task<Portfolio> GetPortfolioAsync(int userId)
        {
            try
            {
                return await _portfolioRepository.GetUserPortfolioAsync(userId);  // Get the user's portfolio
            }
            catch
            {
                throw new ArgumentNullException(nameof(userId), "Portfolio cannot be found");
            }
        }

    }
}

