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
                return (double)portfolio.Positions.Sum(p => p.MarketValue);

            }
            catch
            {
                throw new ArgumentNullException(nameof(userId), "User cannot be null.");
            }
		}

        public async Task<double> GetTotalProfitAndLossAsync(int userId)
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

        public async Task<PortfolioSummary> GetPortfolioSummaryAsync(int userId)
        {
            var positions = await _portfolioRepository.GetPositionsByUserId(userId);
 

            if (positions == null || !positions.Any())
                return new PortfolioSummary { TotalMarketValue = 0, TotalCost = 0, PNL = 0, ReturnPercentage = 0 };

            decimal totalCost = positions.Sum(p => p.Quantity * p.AveragePurchasePrice);
            decimal totalMarketValue = (decimal)positions.Sum(p => p.MarketValue);
            decimal pnl = totalMarketValue - totalCost;
            decimal returnPercentage = totalCost > 0 ? (pnl / totalCost) * 100 : 0;

            return new PortfolioSummary
            {
                TotalMarketValue = totalMarketValue,
                TotalCost = totalCost,
                PNL = pnl,
                ReturnPercentage = returnPercentage
            };
        }

        public async Task<ICollection<Position>> GetAllPositionsAsync(int userId)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var positions = portfolio.Positions;
            return positions;
        }

		public async Task  AddPositionToPortfolioAsync(Position position,int userId)
		{
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position), "Position cannot be null.");
            }

            // Get the portfolio from the repository
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            if (portfolio == null)
            {
                throw new InvalidOperationException("Portfolio not found.");
            }

            // Check if the position exists in the portfolio
            var existingPosition = portfolio.Positions.FirstOrDefault(p => p.StockSymbol == position.StockSymbol);
            if (existingPosition != null)
            {
                // Update the position's quantity and price (or apply any other rule you need)
                existingPosition.Quantity += position.Quantity; // Example: Adding more quantity
                existingPosition.AveragePurchasePrice = position.AveragePurchasePrice; // Replace with your logic if needed
            }
            else
            {
                // If the position doesn't exist, add the new position
                await _portfolioRepository.AddPositionToPortfolioAsync(position, userId);
            }
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

