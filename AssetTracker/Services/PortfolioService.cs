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
                var cashBalance = await GetAvailableFundsAsync(userId);
                // Call performance function with days = 1 (default)
                var performance = await GetPortfolioPerformanceAsync(userId, 1);
                var openPnl = await GetOpenPNLAsync(userId);

                // Store today's market value
                //await _historicalPortfolioValueRepository.StoreMarketValueAsync(userId, DateOnly.FromDateTime(DateTime.Now), totalMarketValue);

                return new PortfolioSummary
                {
                    MarketValue = totalMarketValue,
                    Cost = totalCost,
                    CashBalance = cashBalance,
                    NetAccountValue = (totalMarketValue + cashBalance),
                    OpenPNL = openPnl.PNLValue,
                    OpenReturnPercentage= openPnl.PNLPercentage,
                    DayPNL = performance.PNL,
                    DayReturnPercentage = performance.ReturnPercentage
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating portfolio summary: {ex.Message}");
            }
        }
        private async Task<PnL> GetOpenPNLAsync(Guid userId)
        {
            decimal openPNL = 0;
            decimal totalCostOfOpenPositions = 0;
            var openPositions = await GetPortfolioPositionsAsync(userId);
            foreach (var position in openPositions.Values )
            {
                var summary = await _positionService.GetPositionSummaryAsync(userId, position.Symbol);

                openPNL += summary.OpenPNL;


                totalCostOfOpenPositions += summary.TotalCost;
            }
            decimal openReturnPercentage = 0;
            if (totalCostOfOpenPositions != 0)
            {
                openReturnPercentage = (openPNL / totalCostOfOpenPositions) * 100;
            }
            return new PnL { PNLValue = openPNL, PNLPercentage = openReturnPercentage };
        }
        public async Task<decimal> GetCurrentTotalValue(Guid userId)
        {
            try
            {
                decimal TotalMarketValue = await GetCurrentMarketValue(userId);
                decimal totalCashBalance = await  GetAvailableFundsAsync(userId);
                return TotalMarketValue + totalCashBalance;

            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating Portfolio total value: {ex.Message}");

            }

        }
        public async Task<PortfolioPerformance> GetPortfolioPerformanceAsync(Guid userId, int days)
        {
            try
            {
                var todayTotalValue = await GetCurrentTotalValue(userId);
                var pastTotalValue = await GetTotalValueDaysAgo(userId, days);
                //var todayMarketValue = await GetCurrentMarketValue(userId);
                //var pastMarketValue = await GetMarketValueDaysAgo(userId, days);

                //if (pastMarketValue == null)
                //{
                //    return new PortfolioPerformance
                //    {
                //        PNL = 0,
                //        ReturnPercentage = 0
                //    };
                //}
                if (pastTotalValue == null)
                {
                    return new PortfolioPerformance
                    {
                        PNL = 0,
                        ReturnPercentage = 0
                    };
                }
                //decimal pnl = todayMarketValue - pastMarketValue.Value;
                //decimal returnPercentage = pastMarketValue.Value > 0 ? (pnl / pastMarketValue.Value) * 100 : 0;
                decimal pnl = todayTotalValue - pastTotalValue.Value;

                decimal returnPercentage = pastTotalValue.Value > 0 ? (pnl / pastTotalValue.Value) * 100 : 0;

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
        private async Task<decimal?> GetTotalValueDaysAgo(Guid userId, int days)
        {
            var pastDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-days);
            return await _historicalPortfolioValueRepository.GetTotalValueOnDateAsync(userId, pastDate) ??
                   await GetClosestAvailableTotalValue(userId, pastDate);
        }

        // Try to get the closest available market value within the last 7 days
        private async Task<decimal?> GetClosestAvailableTotalValue(Guid userId, DateOnly requestedDate)
        {
            for (int i = 1; i <= 7; i++) // Try up to a week back
            {
                var adjustedDate = requestedDate.AddDays(-i);
                var value = await _historicalPortfolioValueRepository.GetTotalValueOnDateAsync(userId, adjustedDate);
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

        // Method to store the market value of the portfolio for a specific user
        public async Task StoreTotalValueAsync(Guid userId, decimal marketValue)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            await _historicalPortfolioValueRepository.StoreTotalValueAsync(userId, today, marketValue);
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

            decimal totalMarketValue = await GetCurrentMarketValue(userId);
            var positions = await _portfolioRepository.GetPositionsByUserId(userId);
            foreach (var position in positions.Values)
            {
                // Fetch current price (replace with actual data fetching logic)
                decimal currentPrice = await _alphaVantageStockMarketService.GetStockPriceAsync(position.Symbol);
                position.CurrentPrice = currentPrice;
                position.ComputePositionRatio(totalMarketValue);
            }
            return positions;
        }
        public async Task UpdatePortfolioForAllUsersAsync()
        {
            var users = await _portfolioRepository.GetAllUserIdsAsync(); 
            foreach(var userId in users)
            {
                await UpdatePortfolioByUserId(userId);
            }

        }
        public async Task UpdatePortfolioByUserId(Guid userId)
        {
            try
            {
                decimal totalMarketValue = await GetCurrentMarketValue(userId);
                //var positions = await _portfolioRepository.GetPositionsByUserId(userId);
                var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
                foreach (var position in portfolio.Positions.Values)
                {
                    // Fetch current price (replace with actual data fetching logic)
                    decimal currentPrice = await _alphaVantageStockMarketService.GetStockPriceAsync(position.Symbol);
                    position.CurrentPrice = currentPrice;
                    position.ComputePositionRatio(totalMarketValue);
                }
                await _portfolioRepository.UpdatePortfolioAsync(portfolio);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating {userId}'s Portfolio: {ex.Message}");
            }


        }

        // Method to update market values once a day for all users
        public async Task UpdateTotalValuesForAllUsersAsync()
        {
            var users = await _portfolioRepository.GetAllUserIdsAsync(); 

            foreach (var userId in users)
            {
                //var marketValue = await GetCurrentMarketValue(userId);
                var totalValue = await GetCurrentTotalValue(userId);
                await StoreTotalValueAsync(userId, totalValue);
            }
        }

    }
}
