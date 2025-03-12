using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Repositories;
using AssetTracker.Repositories.Interfaces;
using AssetTracker.Services.Interfaces;

namespace AssetTracker.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IHistoricalPortfolioValueRepository _historicalPortfolioValueRepository;
        private readonly IPositionService _positionService;
        private readonly IAlphaVantageStockMarketService _alphaVantageStockMarketService;

        public PortfolioService(IHistoricalPortfolioValueRepository historicalPortfolioValueRepository,
                                IPortfolioRepository portfolioRepository,
                                IPositionService positionService,
                                IAlphaVantageStockMarketService alphaVantageStockMarketService)
        {
            _portfolioRepository = portfolioRepository;
            _historicalPortfolioValueRepository = historicalPortfolioValueRepository;
            _positionService = positionService;
            _alphaVantageStockMarketService = alphaVantageStockMarketService;
        }

        public async Task<Portfolio> GetUserPortfolioAsync(Guid userId)
        {
            try
            {
                var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
                return portfolio;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching portfolio: {ex.Message}");
            }
        }

        public async Task<PortfolioSummary> GetPortfolioSummaryAsync(Guid userId)
        {
            try
            {
                var totalMarketValue = await GetCurrentMarketValue(userId);
                var totalCost = await GetTotalCost(userId);

                // Call performance function with days = 1 (default)
                var performance = await GetPortfolioPerformanceAsync(userId, 1);

                // Store today's market value
                //await _historicalPortfolioValueRepository.StoreMarketValueAsync(userId, DateOnly.FromDateTime(DateTime.Now), totalMarketValue);

                return new PortfolioSummary
                {
                    TotalMarketValue = totalMarketValue,
                    TotalCost = totalCost,
                    PNL = performance.PNL,
                    ReturnPercentage = performance.ReturnPercentage
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating portfolio summary: {ex.Message}");
            }
        }

        public async Task<PortfolioPerformance> GetPortfolioPerformanceAsync(Guid userId, int days)
        {
            try
            {
                var todayMarketValue = await GetCurrentMarketValue(userId);
                var pastMarketValue = await GetMarketValueDaysAgo(userId, days);

                if (pastMarketValue == null)
                {
                    return new PortfolioPerformance
                    {
                        PNL = 0,
                        ReturnPercentage = 0
                    };
                }

                decimal pnl = todayMarketValue - pastMarketValue.Value;
                decimal returnPercentage = pastMarketValue.Value > 0 ? (pnl / pastMarketValue.Value) * 100 : 0;

                return new PortfolioPerformance
                {
                    PNL = pnl,
                    ReturnPercentage = returnPercentage
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating portfolio performance: {ex.Message}");
            }
        }

        // Fetch market value from a specified number of days ago
        private async Task<decimal?> GetMarketValueDaysAgo(Guid userId, int days)
        {
            var pastDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-days);
            return await _historicalPortfolioValueRepository.GetMarketValueOnDateAsync(userId, pastDate) ??
                   await GetClosestAvailableMarketValue(userId, pastDate);
        }

        // Try to get the closest available market value within the last 7 days
        private async Task<decimal?> GetClosestAvailableMarketValue(Guid userId, DateOnly requestedDate)
        {
            for (int i = 1; i <= 7; i++) // Try up to a week back
            {
                var adjustedDate = requestedDate.AddDays(-i);
                var value = await _historicalPortfolioValueRepository.GetMarketValueOnDateAsync(userId, adjustedDate);
                if (value.HasValue) return value;
            }

            return null; // No valid historical data found
        }

        // Get current market value for all positions in the portfolio
        private async Task<decimal> GetCurrentMarketValue(Guid userId)
        {
            var positions = await _portfolioRepository.GetPositionsByUserId(userId);
            decimal totalMarketValue = 0;

            foreach (var position in positions)
            {
                var positionSummary = await _positionService.GetPositionSummaryAsync(userId, position.Key);
                totalMarketValue += positionSummary.MarketValue;
            }

            return totalMarketValue;
        }

        // Get total cost for all positions in the portfolio
        private async Task<decimal> GetTotalCost(Guid userId)
        {
            var positions = await _portfolioRepository.GetPositionsByUserId(userId);
            decimal totalCost = 0;

            foreach (var position in positions)
            {
                var positionData = position.Value;
                totalCost += positionData.Quantity * positionData.AveragePurchasePrice;
            }

            return totalCost;
        }

        // Update available funds in the portfolio (e.g., deposit/withdraw funds)
        public async Task UpdateAvailableFundsAsync(Guid userId, decimal additionalAmount)
        {
            if (userId == null)
            {
                throw new ArgumentException(nameof(userId), "userId cannot be null.");
            }

            Portfolio portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            if (portfolio.AvailableFunds + additionalAmount < 0)
            {
                throw new ArgumentException(nameof(portfolio.AvailableFunds), "Available funds cannot be negative.");
            }

            portfolio.AvailableFunds += additionalAmount;
            await _portfolioRepository.UpdatePortfolioAsync(portfolio);
        }

        // Get available funds for the user
        public async Task<decimal> GetAvailableFundsAsync(Guid userId)
        {
            if (userId == null)
            {
                throw new ArgumentException(nameof(userId), "userId cannot be null.");
            }

            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            return portfolio.AvailableFunds;
        }

        // Fetch and return all positions for the user, including current prices
        public async Task<Dictionary<string, Position>> GetPortfolioPositionsAsync(Guid userId)
        {
            if (userId == null)
                throw new ArgumentException(nameof(userId), "userId cannot be null.");

            var positions = await _portfolioRepository.GetPositionsByUserId(userId);
            foreach (var position in positions.Values)
            {
                // Fetch current price (replace with actual data fetching logic)
                decimal currentPrice = await _alphaVantageStockMarketService.GetStockPriceAsync(position.Symbol);
                position.CurrentPrice = currentPrice;
            }
            return positions;
        }

        // Method to update market values once a day for all users
        public async Task UpdateMarketValuesForAllUsersAsync()
        {
            var users = await _portfolioRepository.GetAllUserIdsAsync(); // Assuming you have a method to get all user IDs

            foreach (var userId in users)
            {
                var marketValue = await GetCurrentMarketValue(userId);
                await StoreMarketValueAsync(userId, marketValue);
            }
        }

        // Method to store the market value of the portfolio for a specific user
        public async Task StoreMarketValueAsync(Guid userId, decimal marketValue)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            await _historicalPortfolioValueRepository.StoreMarketValueAsync(userId, today, marketValue);
        }
    }
}

//using System;
//using AssetTracker.Models;
//using AssetTracker.Repositories;
//using AssetTracker.Repositories.Interfaces;
//using AssetTracker.Repositories.MongoDBRepositories;
//using AssetTracker.Services.Interfaces;


//namespace AssetTracker.Services
//{
//	public class PortfolioService:IPortfolioService
//	{
//        private readonly IPortfolioRepository _portfolioRepository;
//        private readonly IHistoricalPortfolioValueRepository _historicalPortfolioValueRepository;
//        private readonly IPositionService _positionService;
//        private readonly IAlphaVantageStockMarketService _alphaVantageStockMarketService;

//        //private readonly IPositionService _positionService;
//        //private readonly IP
//        public PortfolioService( IHistoricalPortfolioValueRepository historicalPortfolioValueRepository, IPortfolioRepository portfolioRepository, IPositionService positionService, IAlphaVantageStockMarketService alphaVantageStockMarketService)
//        {
//            _portfolioRepository = portfolioRepository;
//            _historicalPortfolioValueRepository = historicalPortfolioValueRepository;
//            _positionService = positionService;
//            _alphaVantageStockMarketService = alphaVantageStockMarketService;
//        }
//        public async Task<Portfolio> GetUserPortfolioAsync(Guid userId)
//        {
//            try
//            {
//                var portfolio =  await _portfolioRepository.GetUserPortfolioAsync(userId);
//                return portfolio;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Error fetching portfolio: {ex.Message}");

//            }
//        }

//        public async Task<PortfolioSummary> GetPortfolioSummaryAsync(Guid userId)
//        {
//            try
//            {
//                var totalMarketValue = await GetCurrentMarketValue(userId);
//                var totalCost = await GetTotalCost(userId);


//                // Call performance function with days = 1 (default)
//                var performance = await GetPortfolioPerformanceAsync(userId, 1);
//                await _historicalPortfolioValueRepository.StoreMarketValueAsync(userId, DateOnly.FromDateTime(DateTime.Now), totalMarketValue);

//                return new PortfolioSummary
//                {
//                    TotalMarketValue = totalMarketValue,
//                    TotalCost = totalCost,
//                    PNL = performance.PNL,
//                    ReturnPercentage = performance.ReturnPercentage
//                };

//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Error calculating portfolio summary: {ex.Message}");
//            }
//        }
//        public async Task<PortfolioPerformance> GetPortfolioPerformanceAsync(Guid userId, int days)
//        {
//            try
//            {
//                var todayMarketValue = await GetCurrentMarketValue(userId);
//                var pastMarketValue = await GetMarketValueDaysAgo(userId, days);

//                if (pastMarketValue == null)
//                {
//                    //throw new Exception($"Not enough historical data to calculate {days}-day performance.");

//                    return new PortfolioPerformance
//                    {
//                        PNL = 0,
//                        ReturnPercentage = 0
//                    };
//                }

//                decimal pnl = todayMarketValue - pastMarketValue.Value;
//                decimal returnPercentage = pastMarketValue.Value > 0 ? (pnl / pastMarketValue.Value) * 100 : 0;

//                return new PortfolioPerformance
//                {
//                    PNL = pnl,
//                    ReturnPercentage = returnPercentage
//                };
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Error calculating portfolio performance: {ex.Message}");
//            }
//        }


//        private async Task<decimal?> GetMarketValueDaysAgo(Guid userId, int days)
//        {
//            var pastDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-days);
//            return await _historicalPortfolioValueRepository.GetMarketValueOnDateAsync(userId, pastDate) ??
//                   await GetClosestAvailableMarketValue(userId, pastDate);
//        }
//        private async Task<decimal?> GetClosestAvailableMarketValue(Guid userId, DateOnly requestedDate)
//        {
//            for (int i = 1; i <= 7; i++) // Try up to a week back
//            {
//                var adjustedDate = requestedDate.AddDays(-i);
//                var value = await _historicalPortfolioValueRepository.GetMarketValueOnDateAsync(userId, adjustedDate);
//                if (value.HasValue) return value;
//            }

//            return null; // No valid historical data found
//        }

//        private async Task<decimal> GetCurrentMarketValue(Guid userId)
//        {
//            var positions = await _portfolioRepository.GetPositionsByUserId(userId);
//            decimal totalMarketValue = 0;

//            foreach (var position in positions)
//            {
//                var positionSummary = await _positionService.GetPositionSummaryAsync(userId, position.Key);
//                totalMarketValue += positionSummary.MarketValue;
//            }

//            return totalMarketValue;
//        }

//        private async Task<decimal> GetTotalCost(Guid userId)
//        {
//            var positions = await _portfolioRepository.GetPositionsByUserId(userId);
//            decimal totalCost = 0;

//            foreach (var position in positions)
//            {
//                var positionData = position.Value;
//                totalCost += positionData.Quantity * positionData.AveragePurchasePrice;
//            }

//            return totalCost;
//        }

//        public async Task UpdateAvailableFundsAsync(Guid userId, decimal additionalAmount)
//        {
//            if(userId == null)
//            {
//                throw new ArgumentException(nameof(userId), "userId cannot be null.");
//            }
//            Portfolio portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
//            if (portfolio.AvailableFunds+additionalAmount < 0)
//            {
//                throw new ArgumentException(nameof(portfolio.AvailableFunds), "Available funds cannot be negative.");

//            }

//            portfolio.AvailableFunds += additionalAmount;


//            await _portfolioRepository.UpdatePortfolioAsync(portfolio);
//        }

//        public async Task<decimal> GetAvailableFundsAsync(Guid userId)
//        {
//            if (userId == null)
//            {
//                throw new ArgumentException(nameof(userId), "userId cannot be null.");
//            }

//            var portfolio =  await _portfolioRepository.GetUserPortfolioAsync(userId);
//            return portfolio.AvailableFunds;
//        }
//        public async Task<Dictionary<string, Position>> GetPortfolioPositionsAsync(Guid userId)
//        {
//            if(userId == null)
//                throw new ArgumentException(nameof(userId), "userId cannot be null.");
//            var positions = await _portfolioRepository.GetPositionsByUserId(userId);
//            foreach (var position in positions.Values)
//            {
//                // Fetch current price (replace with actual data fetching logic)
//                decimal currentPrice = await _alphaVantageStockMarketService.GetStockPriceAsync(position.Symbol);

//                position.CurrentPrice = currentPrice;
//            }
//            return positions;

//        }

//        //public async Task DepositFundsAsync(Guid userId, decimal depositedAmount)
//        //{
//        //   var portfolio =  await _portfolioRepository.GetUserPortfolioAsync(userId);
//        //    portfolio.AvailableFunds += depositedAmount;

//        //    await _portfolioRepository.UpdatePortfolioAsync(portfolio);
//        //}
//        //public async Task WithdrawnFundsAsync(Guid userId, decimal withdrawnAmount)
//        //{

//        //    var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
//        //    portfolio.AvailableFunds -= withdrawnAmount;

//        //    await _portfolioRepository.UpdatePortfolioAsync(portfolio);
//        //}

//        //      public async Task<ICollection<Position>> GetAllPositionsAsync(Guid userId)
//        //      {
//        //          var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
//        //          var positions = portfolio.Positions;
//        //          return positions;
//        //      }

//        //public async Task  AddPositionToPortfolioAsync(Position position,Guid userId)
//        //{
//        //          if (position == null)
//        //              throw new ArgumentNullException(nameof(position), "Position cannot be null.");

//        //          var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
//        //          if (portfolio == null)
//        //              throw new InvalidOperationException("Portfolio not found.");

//        //          var existingPosition = portfolio.Positions.FirstOrDefault(p => p.StockSymbol == position.StockSymbol);
//        //          if (existingPosition != null)
//        //          {
//        //              var additionalQuantity = position.Quantity;
//        //              existingPosition.Quantity += additionalQuantity;
//        //              existingPosition.AveragePurchasePrice = position.AveragePurchasePrice;

//        //              await _positionService.AddPositionHistoryAsync(new PositionHistory
//        //              {
//        //                  UserId = userId,
//        //                  PositionId = existingPosition.PositionId,
//        //                  Symbol = existingPosition.StockSymbol,
//        //                  TransactionDate = DateTime.UtcNow,
//        //                  ActionType = "BUY",
//        //                  Quantity = additionalQuantity,
//        //                  Price = position.AveragePurchasePrice
//        //              });
//        //          }
//        //          else
//        //          {
//        //              await _portfolioRepository.AddPositionToPortfolioAsync(position, userId);

//        //              await _positionService.AddPositionHistoryAsync(new PositionHistory
//        //              {
//        //                  UserId = userId,
//        //                  PositionId = position.PositionId,
//        //                  Symbol = position.StockSymbol,
//        //                  TransactionDate = DateTime.UtcNow,
//        //                  ActionType = "BUY",
//        //                  Quantity = position.Quantity,
//        //                  Price = position.AveragePurchasePrice
//        //              });
//        //          }
//        //      }

//        //      public async Task RemovePositionAsync(Guid userId, string symbol) 
//        //{
//        //          try
//        //          {
//        //              var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
//        //              var position = portfolio.Positions.FirstOrDefault(p => p.StockSymbol == symbol);
//        //              if (position != null)
//        //              {
//        //                  await _positionService.AddPositionHistoryAsync(new PositionHistory
//        //                  {
//        //                      UserId = userId,
//        //                      PositionId = position.PositionId,
//        //                      Symbol = symbol,
//        //                      TransactionDate = DateTime.UtcNow,
//        //                      ActionType = "SELL",
//        //                      Quantity = position.Quantity,
//        //                      Price = (decimal)position.Stock.CurrentPrice
//        //                  }) ;
//        //              }

//        //              await _portfolioRepository.RemovePositionFromPortfolioAsync(userId, symbol);
//        //          }
//        //          catch
//        //          {
//        //              throw new Exception("Position not found");
//        //          }
//        //      }

//        //      public async Task<Portfolio> GetPortfolioAsync(Guid userId)
//        //      {
//        //          try
//        //          {
//        //              return await _portfolioRepository.GetUserPortfolioAsync(userId);  // Get the user's portfolio
//        //          }
//        //          catch
//        //          {
//        //              throw new ArgumentNullException(nameof(userId), "Portfolio cannot be found");
//        //          }
//        //      }

//    }
//}

