using System;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services
{
	public class PortfolioService:IPortfolioService
	{
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IPositionService _positionService;

        //private readonly IPositionService _positionService;
        //private readonly IP
        public PortfolioService(IPortfolioRepository portfolioRepository, IPositionService positionService)
        {
            _portfolioRepository = portfolioRepository;
            _positionService = positionService;
        }


        public async Task<PortfolioSummary> GetPortfolioSummaryAsync(Guid userId)
        {
            try
            {
                var totalMarketValue = await GetCurrentMarketValue(userId);
                var totalCost = await GetTotalCost(userId);


                // Call performance function with days = 1 (default)
                var performance = await GetPortfolioPerformanceAsync(userId, 1);
                await _portfolioRepository.StoreMarketValueAsync(userId, DateOnly.FromDateTime(DateTime.Now), totalMarketValue);

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
                    //throw new Exception($"Not enough historical data to calculate {days}-day performance.");

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
        private async Task<decimal?> GetMarketValueDaysAgo(Guid userId, int days)
        {
            var pastDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-days);
            return await _portfolioRepository.GetMarketValueOnDateAsync(userId, pastDate) ??
                   await GetClosestAvailableMarketValue(userId, pastDate);
        }
        private async Task<decimal?> GetClosestAvailableMarketValue(Guid userId, DateOnly requestedDate)
        {
            for (int i = 1; i <= 7; i++) // Try up to a week back
            {
                var adjustedDate = requestedDate.AddDays(-i);
                var value = await _portfolioRepository.GetMarketValueOnDateAsync(userId, adjustedDate);
                if (value.HasValue) return value;
            }

            return null; // No valid historical data found
        }

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

        public async Task UpdateAvailableFundsAsync(Guid userId, decimal additionalAmount)
        {
            if(userId == null)
            {
                throw new ArgumentException(nameof(userId), "userId cannot be null.");
            }
            Portfolio portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            if (portfolio.AvailableFunds+additionalAmount < 0)
            {
                throw new ArgumentException(nameof(portfolio.AvailableFunds), "Available funds cannot be negative.");

            }

            portfolio.AvailableFunds += additionalAmount;

            
            await _portfolioRepository.UpdatePortfolioAsync(portfolio);
        }

        public async Task<decimal> GetAvailableFundsAsync(Guid userId)
        {
            if (userId == null)
            {
                throw new ArgumentException(nameof(userId), "userId cannot be null.");
            }

            var portfolio =  await _portfolioRepository.GetUserPortfolioAsync(userId);
            return portfolio.AvailableFunds;
        }

        //public async Task DepositFundsAsync(Guid userId, decimal depositedAmount)
        //{
        //   var portfolio =  await _portfolioRepository.GetUserPortfolioAsync(userId);
        //    portfolio.AvailableFunds += depositedAmount;

        //    await _portfolioRepository.UpdatePortfolioAsync(portfolio);
        //}
        //public async Task WithdrawnFundsAsync(Guid userId, decimal withdrawnAmount)
        //{

        //    var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
        //    portfolio.AvailableFunds -= withdrawnAmount;

        //    await _portfolioRepository.UpdatePortfolioAsync(portfolio);
        //}

        //      public async Task<ICollection<Position>> GetAllPositionsAsync(Guid userId)
        //      {
        //          var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
        //          var positions = portfolio.Positions;
        //          return positions;
        //      }

        //public async Task  AddPositionToPortfolioAsync(Position position,Guid userId)
        //{
        //          if (position == null)
        //              throw new ArgumentNullException(nameof(position), "Position cannot be null.");

        //          var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
        //          if (portfolio == null)
        //              throw new InvalidOperationException("Portfolio not found.");

        //          var existingPosition = portfolio.Positions.FirstOrDefault(p => p.StockSymbol == position.StockSymbol);
        //          if (existingPosition != null)
        //          {
        //              var additionalQuantity = position.Quantity;
        //              existingPosition.Quantity += additionalQuantity;
        //              existingPosition.AveragePurchasePrice = position.AveragePurchasePrice;

        //              await _positionService.AddPositionHistoryAsync(new PositionHistory
        //              {
        //                  UserId = userId,
        //                  PositionId = existingPosition.PositionId,
        //                  Symbol = existingPosition.StockSymbol,
        //                  TransactionDate = DateTime.UtcNow,
        //                  ActionType = "BUY",
        //                  Quantity = additionalQuantity,
        //                  Price = position.AveragePurchasePrice
        //              });
        //          }
        //          else
        //          {
        //              await _portfolioRepository.AddPositionToPortfolioAsync(position, userId);

        //              await _positionService.AddPositionHistoryAsync(new PositionHistory
        //              {
        //                  UserId = userId,
        //                  PositionId = position.PositionId,
        //                  Symbol = position.StockSymbol,
        //                  TransactionDate = DateTime.UtcNow,
        //                  ActionType = "BUY",
        //                  Quantity = position.Quantity,
        //                  Price = position.AveragePurchasePrice
        //              });
        //          }
        //      }

        //      public async Task RemovePositionAsync(Guid userId, string symbol) 
        //{
        //          try
        //          {
        //              var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
        //              var position = portfolio.Positions.FirstOrDefault(p => p.StockSymbol == symbol);
        //              if (position != null)
        //              {
        //                  await _positionService.AddPositionHistoryAsync(new PositionHistory
        //                  {
        //                      UserId = userId,
        //                      PositionId = position.PositionId,
        //                      Symbol = symbol,
        //                      TransactionDate = DateTime.UtcNow,
        //                      ActionType = "SELL",
        //                      Quantity = position.Quantity,
        //                      Price = (decimal)position.Stock.CurrentPrice
        //                  }) ;
        //              }

        //              await _portfolioRepository.RemovePositionFromPortfolioAsync(userId, symbol);
        //          }
        //          catch
        //          {
        //              throw new Exception("Position not found");
        //          }
        //      }

        //      public async Task<Portfolio> GetPortfolioAsync(Guid userId)
        //      {
        //          try
        //          {
        //              return await _portfolioRepository.GetUserPortfolioAsync(userId);  // Get the user's portfolio
        //          }
        //          catch
        //          {
        //              throw new ArgumentNullException(nameof(userId), "Portfolio cannot be found");
        //          }
        //      }

    }
}

