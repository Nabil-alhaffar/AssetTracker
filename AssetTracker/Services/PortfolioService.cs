using System;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services
{
	public class PortfolioService:IPortfolioService
	{
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IStockService _stockService;  // To get the market price of the stock
        private readonly IPositionService _positionService;

        //private readonly IPositionService _positionService;
        //private readonly IP
        public PortfolioService(IPortfolioRepository portfolioRepository, IStockService stockService, IPositionService positionService)
        {
            _portfolioRepository = portfolioRepository;
            _stockService = stockService;
            _positionService = positionService;
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
            try
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
            catch 
            {
                throw new Exception("User Positions not found");
            }

            

            
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
                throw new ArgumentNullException(nameof(position), "Position cannot be null.");

            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            if (portfolio == null)
                throw new InvalidOperationException("Portfolio not found.");

            var existingPosition = portfolio.Positions.FirstOrDefault(p => p.StockSymbol == position.StockSymbol);
            if (existingPosition != null)
            {
                var additionalQuantity = position.Quantity;
                existingPosition.Quantity += additionalQuantity;
                existingPosition.AveragePurchasePrice = position.AveragePurchasePrice;

                await _positionService.AddPositionHistoryAsync(new PositionHistory
                {
                    UserId = userId,
                    PositionId = existingPosition.Id,
                    Symbol = existingPosition.StockSymbol,
                    TransactionDate = DateTime.UtcNow,
                    ActionType = "BUY",
                    Quantity = additionalQuantity,
                    Price = position.AveragePurchasePrice
                });
            }
            else
            {
                await _portfolioRepository.AddPositionToPortfolioAsync(position, userId);

                await _positionService.AddPositionHistoryAsync(new PositionHistory
                {
                    UserId = userId,
                    PositionId = position.Id,
                    Symbol = position.StockSymbol,
                    TransactionDate = DateTime.UtcNow,
                    ActionType = "BUY",
                    Quantity = position.Quantity,
                    Price = position.AveragePurchasePrice
                });
            }
        }

        public async Task RemovePositionAsync(int userId, string symbol) 
		{
            try
            {
                var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
                var position = portfolio.Positions.FirstOrDefault(p => p.StockSymbol == symbol);
                if (position != null)
                {
                    await _positionService.AddPositionHistoryAsync(new PositionHistory
                    {
                        UserId = userId,
                        PositionId = position.Id,
                        Symbol = symbol,
                        TransactionDate = DateTime.UtcNow,
                        ActionType = "SELL",
                        Quantity = position.Quantity,
                        Price = (decimal)position.Stock.CurrentPrice
                    }) ;
                }

                await _portfolioRepository.RemovePositionFromPortfolioAsync(userId, symbol);
            }
            catch
            {
                throw new Exception("Position not found");
            }
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

