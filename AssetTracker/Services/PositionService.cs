using System;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Repositories;
using System.Collections.Generic;
using static AssetTracker.Models.Position;

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

        public async Task UpdatePositionAsync(Order order)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(order.UserId);
            if (portfolio == null)
                throw new Exception("Portfolio not found");

            var position = portfolio.Positions.FirstOrDefault(p => p.Key == order.Symbol).Value;

            if (position == null)
            {
                // Create a new position with the appropriate type (Long or Short)
                position = new Position
                {
                    UserId = order.UserId,
                    Symbol = order.Symbol,
                    Quantity = order.Type == OrderType.Short ? -order.Quantity : order.Quantity,
                    AveragePurchasePrice = order.Price,
                    Type = order.Type == OrderType.Short ? PositionType.Short : PositionType.Long
                };
                portfolio.Positions.Add(order.Symbol, position);
            }
            else
            {
                // Modify existing position
                if (position.Type == PositionType.Short && order.Type == OrderType.Buy)
                {
                    throw new InvalidOperationException("Cannot buy long while holding a short position. Close the short position first.");
                }
                if (position.Type == PositionType.Long && order.Type == OrderType.Short)
                {
                    throw new InvalidOperationException("Cannot short sell while holding a long position. Sell the long position first.");
                }

                decimal newQuantity;
                if (order.Type == OrderType.Sell && position.Type == PositionType.Long)
                {
                    // Selling long position, reduce the quantity
                    newQuantity = position.Quantity - order.Quantity;
                }
                else if (order.Type == OrderType.CloseShort && position.Type == PositionType.Short)
                {
                    // Closing short position, add the quantity back to the short position
                    newQuantity = position.Quantity + order.Quantity;
                }
                else
                {
                    newQuantity = position.Quantity + (position.Type == PositionType.Short ? -order.Quantity : order.Quantity);
                }



                if (newQuantity == 0)
                {
                    portfolio.Positions.Remove(order.Symbol); // Position closed
                }
                else
                {
                    position.AveragePurchasePrice = ((position.AveragePurchasePrice * Math.Abs(position.Quantity)) + (order.Price * order.Quantity)) / Math.Abs(newQuantity);
                    position.Quantity = newQuantity;
                }
            }

            await _portfolioRepository.UpdatePortfolioAsync(portfolio);

            // ✅ Determine action type for history logging
            string actionType = order.Type switch
            {
                OrderType.Buy => "BUY",
                OrderType.Sell => "SELL",
                OrderType.Short => "SELL SHORT",
                OrderType.CloseShort => "BUY TO CLOSE SHORT",
                _ => "UNKNOWN"
            };

            await AddPositionHistoryAsync(new PositionHistory
            {
                UserId = order.UserId,
                PositionId = position.PositionId,
                Symbol = order.Symbol,
                TransactionDate = DateTime.UtcNow,
                ActionType = actionType,
                Quantity = Math.Abs(order.Quantity),
                Price = order.Price
            });
        }

        //public async Task AddOrUpdatePositionAsync(Guid userId, string symbol, decimal quantity, decimal price)
        //{
        //    var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
        //    var position = portfolio.Positions.FirstOrDefault(p => p.Key == symbol).Value;

        //    if (position == null)
        //    {
        //        position = new Position { UserId = userId, Symbol = symbol, Quantity = quantity, AveragePurchasePrice = price, PositionType = Position.PositionType.Long };
        //        portfolio.Positions.Add(symbol, position);
        //    }
        //    else
        //    {
        //        position.AveragePurchasePrice = ((position.AveragePurchasePrice * position.Quantity) + (price * quantity)) / (position.Quantity + quantity);
        //        position.Quantity += quantity;
        //    }

        //    // Persist the changes back to the repository
        //    await _portfolioRepository.UpdatePortfolioAsync(portfolio);  // Save updated portfolio

        //    // Add position history for this action
        //    await AddPositionHistoryAsync(new PositionHistory
        //    {
        //        UserId = userId,
        //        PositionId = position.PositionId,
        //        Symbol = position.Symbol,
        //        TransactionDate = DateTime.UtcNow,
        //        ActionType = "BUY",
        //        Quantity = quantity,
        //        Price = price
        //    });
        //}

        //// Reduce or remove a position from the portfolio
        //public async Task ReduceOrRemovePositionAsync(Guid userId, string symbol, decimal quantity, decimal price)
        //{
        //    var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
        //    var position = portfolio.Positions.FirstOrDefault(p => p.Key == symbol).Value;

        //    if (position == null)
        //    {
        //        return;  // Position not found
        //    }

        //    bool shortPosition = position.PositionType == Position.PositionType.Short ? true : false;
        //    bool fullClose = (quantity == position.Quantity);
        //    // Add position history for this action
        //    await AddPositionHistoryAsync(new PositionHistory
        //    {
        //        UserId = userId,
        //        PositionId = position.PositionId,
        //        Symbol = symbol,
        //        TransactionDate = DateTime.UtcNow,
        //        ActionType = (shortPosition ? $"BUY TO CLOSE SHORT:" : "SELL TO CLOSE: ") + $"{quantity}/{position.Quantity}",
        //        Quantity = quantity,
        //        Price = price
        //    });

        //    position.Quantity -= quantity;

        //    if (position.Quantity == 0)
        //    {
        //        portfolio.Positions.Remove(symbol);  // Remove position if quantity is zero
        //    }

        //    await _portfolioRepository.UpdatePortfolioAsync(portfolio);  // Save updated portfolio
        //}

        //// Add or update a short position in the portfolio
        //public async Task AddOrUpdateShortPositionAsync(Guid userId, string symbol, decimal quantity, decimal price)
        //{
        //    var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
        //    var position = portfolio.Positions.FirstOrDefault(p => p.Key == symbol).Value;

        //    // Add position history for this action
        //    await AddPositionHistoryAsync(new PositionHistory
        //    {
        //        UserId = userId,
        //        PositionId = position?.PositionId ?? Guid.NewGuid(),  // If position doesn't exist, generate new Id
        //        Symbol = symbol,
        //        TransactionDate = DateTime.UtcNow,
        //        ActionType = "SELL SHORT",
        //        Quantity = quantity,
        //        Price = price
        //    });

        //    if (position == null)
        //    {
        //        position = new Position { UserId = userId, Symbol = symbol, Quantity = -quantity, AveragePurchasePrice = price, PositionType = Position.PositionType.Short };
        //        portfolio.Positions.Add(symbol, position);
        //    }
        //    else
        //    {
        //        position.AveragePurchasePrice = ((position.AveragePurchasePrice * position.Quantity) + (price * -quantity)) / (position.Quantity - quantity);
        //        position.Quantity -= quantity;
        //    }

        //    await _portfolioRepository.UpdatePortfolioAsync(portfolio);  // Save updated portfolio
        //}
    }
}
