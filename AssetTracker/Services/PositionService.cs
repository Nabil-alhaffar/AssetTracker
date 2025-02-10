using System;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Repositories;
using System.Collections.Generic;

namespace AssetTracker.Services
{
    public class PositionService : IPositionService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IAlphaVantageStockMarketService _alphaVantageStockMarketService; // Added to get current stock price
        private readonly Dictionary<Guid, List<PositionHistory>> _positionHistoryStorage = new();

        // Constructor
        public PositionService(IPortfolioRepository portfolioRepository, IAlphaVantageStockMarketService alphaVantageStockMarketService)
        {
            _portfolioRepository = portfolioRepository;
            _alphaVantageStockMarketService = alphaVantageStockMarketService;
        }

        // Split a position based on the split factor
        public async Task SplitPositionAsync(Guid userId, string symbol, int splitFactor)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Key == symbol).Value;

            if (position != null)
            {
                // Adjust the quantity and average purchase price according to the split factor
                position.Quantity *= splitFactor;
                position.AveragePurchasePrice /= splitFactor;

                // Persist the changes back to the repository
                await _portfolioRepository.UpdatePortfolioAsync(portfolio);  // Save updated portfolio
            }
        }

        // Check if a position has triggered the stop loss
        public async Task<bool> CheckPositionForStopLossAsync(Guid userId, string symbol, decimal stopLossPrice)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Key == symbol).Value;

            if (position == null)
            {
                throw new InvalidOperationException("Position not found.");
            }

            // Get the current price of the stock from an external service
            decimal currentPrice = await _alphaVantageStockMarketService.GetStockPriceAsync(symbol);

            // Compare the current market price of the stock with the stop-loss price
            if (currentPrice <= stopLossPrice)
            {
                return true;  // Stop loss triggered
            }

            return false;  // Stop loss not triggered
        }

        // Update Profit & Loss for a given position
        public async Task UpdatePositionProfitLossAsync(Guid userId, string symbol)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Key == symbol).Value;

            if (position == null)
            {
                throw new InvalidOperationException("Position not found.");
            }

            // Get the current price of the stock from an external service
            decimal currentPrice = await _alphaVantageStockMarketService.GetStockPriceAsync(symbol);

            // Persist the changes back to the repository
            await _portfolioRepository.UpdatePortfolioAsync(portfolio);  // Save updated portfolio
        }

        // Add or update a position in the portfolio
        public async Task AddOrUpdatePositionAsync(Guid userId, string symbol, decimal quantity, decimal price)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Key == symbol).Value;

            if (position == null)
            {
                position = new Position { UserId = userId, Symbol = symbol, Quantity = quantity, AveragePurchasePrice = price };
                portfolio.Positions.Add(symbol, position);
            }
            else
            {
                position.AveragePurchasePrice = ((position.AveragePurchasePrice * position.Quantity) + (price * quantity)) / (position.Quantity + quantity);
                position.Quantity += quantity;
            }

            // Persist the changes back to the repository
            await _portfolioRepository.UpdatePortfolioAsync(portfolio);  // Save updated portfolio

            // Add position history for this action
            await AddPositionHistoryAsync(new PositionHistory
            {
                UserId = userId,
                PositionId = position.PositionId,
                Symbol = position.Symbol,
                TransactionDate = DateTime.UtcNow,
                ActionType = "BUY",
                Quantity = quantity,
                Price = price
            });
        }

        // Reduce or remove a position from the portfolio
        public async Task ReduceOrRemovePositionAsync(Guid userId, string symbol, decimal quantity, decimal price)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Key == symbol).Value;

            if (position == null)
            {
                return;  // Position not found
            }

            bool shortPosition = position.Quantity < 0;

            // Add position history for this action
            await AddPositionHistoryAsync(new PositionHistory
            {
                UserId = userId,
                PositionId = position.PositionId,
                Symbol = symbol,
                TransactionDate = DateTime.UtcNow,
                ActionType = shortPosition ? "BUY TO CLOSE SHORT" : "SELL",
                Quantity = quantity,
                Price = price
            });

            position.Quantity -= quantity;

            if (position.Quantity == 0)
            {
                portfolio.Positions.Remove(symbol);  // Remove position if quantity is zero
            }

            await _portfolioRepository.UpdatePortfolioAsync(portfolio);  // Save updated portfolio
        }

        // Add or update a short position in the portfolio
        public async Task AddOrUpdateShortPositionAsync(Guid userId, string symbol, decimal quantity, decimal price)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Key == symbol).Value;

            // Add position history for this action
            await AddPositionHistoryAsync(new PositionHistory
            {
                UserId = userId,
                PositionId = position?.PositionId ?? Guid.NewGuid(),  // If position doesn't exist, generate new Id
                Symbol = symbol,
                TransactionDate = DateTime.UtcNow,
                ActionType = "SELL SHORT",
                Quantity = quantity,
                Price = price
            });

            if (position == null)
            {
                position = new Position { UserId = userId, Symbol = symbol, Quantity = -quantity, AveragePurchasePrice = price };
                portfolio.Positions.Add(symbol, position);
            }
            else
            {
                position.AveragePurchasePrice = ((position.AveragePurchasePrice * position.Quantity) + (price * -quantity)) / (position.Quantity - quantity);
                position.Quantity -= quantity;
            }

            await _portfolioRepository.UpdatePortfolioAsync(portfolio);  // Save updated portfolio
        }

        // Add position history
        public async Task AddPositionHistoryAsync(PositionHistory history)
        {
            if (!_positionHistoryStorage.ContainsKey(history.UserId))
            {
                _positionHistoryStorage[history.UserId] = new List<PositionHistory>();
            }

             _positionHistoryStorage[history.UserId].Add(history);
        }

        public async Task<List<PositionHistory>> GetPositionHistoryAsync(Guid userId, string symbol)
        {
            if (!_positionHistoryStorage.TryGetValue(userId, out var history))
            {
                return new List<PositionHistory>();
            }

            var filteredHistory = string.IsNullOrEmpty(symbol)
                ? history
                : history.Where(h => h.Symbol == symbol).ToList();

            return filteredHistory.OrderByDescending(h => h.TransactionDate).ToList();
        }


        // Get PositionSummary for a user and symbol
        public async Task<PositionSummary> GetPositionSummaryAsync(Guid userId, string symbol)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Key == symbol).Value;

            if (position != null)
            {
                // Get the current price from stock service
                decimal currentPrice = await _alphaVantageStockMarketService.GetStockPriceAsync(symbol);

                // Create and return PositionSummary
                return new PositionSummary
                {
                    PositionId = position.PositionId,
                    Symbol = position.Symbol,
                    Quantity = position.Quantity,
                    AveragePurchasePrice = position.AveragePurchasePrice,
                    CurrentPrice = currentPrice
                };
            }

            return null; // Return null if position not found
        }

            public async Task<Position> GetPositionAsync(Guid userId, string symbol)
        {
            return await _portfolioRepository.GetUserPositionBySymbol(userId, symbol);

        }
    }
}
