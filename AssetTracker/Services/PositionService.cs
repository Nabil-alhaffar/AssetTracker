using System;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Repositories;
using System.Collections.Generic;
//using static AssetTracker.Models.Position;
using AssetTracker.Services.Interfaces;
using AssetTracker.Repositories.Interfaces;
using static AssetTracker.Models.Position;
using AssetTracker.Repositories.MongoDBRepositories;

namespace AssetTracker.Services
{
    public class PositionService : IPositionService
    {
        private readonly ILogger<PositionService> _logger;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IAlphaVantageStockMarketService _alphaVantageStockMarketService; // Added to get current stock price
        private readonly Dictionary<Guid, List<PositionHistory>> _positionHistoryStorage = new();

        // Constructor
        public PositionService(ILogger<PositionService> logger, IPortfolioRepository portfolioRepository, IAlphaVantageStockMarketService alphaVantageStockMarketService)
        {
            _portfolioRepository = portfolioRepository;
            _alphaVantageStockMarketService = alphaVantageStockMarketService;
            _logger = logger;
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
                //decimal currentPrice = await _alphaVantageStockMarketService.GetStockPriceAsync(symbol);
                //position.CurrentPrice = currentPrice;
                var pnl = await GetPositionPnL(position);

                //_logger.LogInformation($"Fetched price for {symbol}: {currentPrice}");

                // Create and return PositionSummary
                return new PositionSummary
                {
                    PositionId = position.PositionId,
                    Symbol = position.Symbol,
                    Quantity = position.Quantity,
                    AveragePurchasePrice = position.AveragePurchasePrice,
                    CurrentPrice = position.CurrentPrice,
                    OpenPNL = pnl.PNLValue,
                    OpenPNLPercentage = pnl.PNLPercentage

                };
            }

            return null; // Return null if position not found
        }
        private async Task<PnL> GetPositionPnL (Position position)
        {
            decimal openPNL = 0;

            decimal currentPrice = position.CurrentPrice; // Fetch current market price for the symbol
            decimal averageEntryPrice = position.AveragePurchasePrice;
            decimal quantity = position.Quantity;

            if (position.Type == Position.PositionType.Long) // Long position
            {
                openPNL += (currentPrice - averageEntryPrice) * quantity;
            }
            else if (position.Type == Position.PositionType.Short) // Short position
            {
                openPNL += (averageEntryPrice - currentPrice) * Math.Abs(quantity);
            }


            decimal openPnlPercentage = 0;
            // Calculate OpenReturnPercentage

            if (position.TotalCost != 0)
            {
                openPnlPercentage = (openPNL / Math.Abs(position.TotalCost)) * 100;
            }

            return new PnL
            {
                PNLValue = openPNL,
                PNLPercentage = openPnlPercentage,
            };
        }

        public async Task<Position> GetPositionAsync(Guid userId, string symbol)
        {
            return await _portfolioRepository.GetUserPositionBySymbol(userId, symbol);

        }

        public async Task UpdatePositionAsync(Order order)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(order.UserId);
            if (portfolio == null)
                throw new KeyNotFoundException($"No portfolio found for user {order.UserId}");

            var positions = portfolio.Positions;
            // 2) Try to get existing position
            if (!positions.TryGetValue(order.Symbol, out var position))
            {
                // 2a) If no position exists, only Buy or Short can create one
                if (order.Type == OrderType.Sell || order.Type == OrderType.CloseShort)
                    throw new InvalidOperationException($"Cannot {order.Type} when no existing {order.Symbol} position.");

                var qty = (order.Type == OrderType.Short ? -order.Quantity : order.Quantity);
                position = new Position
                {
                    UserId = order.UserId,
                    Symbol = order.Symbol,
                    Quantity = qty,
                    AveragePurchasePrice = order.Price,
                    CurrentPrice = order.Price,
                    Type = order.Type == OrderType.Short
                                           ? PositionType.Short
                                           : PositionType.Long
                };
                positions[order.Symbol] = position;
            }
            else
            {
                // 3) Existing position: enforce exclusivity
                if (position.Type == PositionType.Short && order.Type == OrderType.Buy)
                    throw new InvalidOperationException("Must close short before buying long.");
                if (position.Type == PositionType.Long && order.Type == OrderType.Short)
                    throw new InvalidOperationException("Must sell long before shorting.");

                // 4) Apply the fill
                switch (order.Type)
                {
                    case OrderType.Buy:
                        // increase long
                        position.AveragePurchasePrice =
                            ((position.AveragePurchasePrice * position.Quantity)
                             + (order.Price * order.Quantity))
                            / (position.Quantity + order.Quantity);
                        position.Quantity += order.Quantity;
                        break;

                    case OrderType.Sell:
                        // decrease long
                        position.Quantity -= order.Quantity;
                        break;

                    case OrderType.Short:
                        // increase short (more negative)
                        position.AveragePurchasePrice =
                            ((Math.Abs(position.Quantity) * position.AveragePurchasePrice)
                             + (order.Price * order.Quantity))
                            / (Math.Abs(position.Quantity) + order.Quantity);
                        position.Quantity -= order.Quantity;  // e.g. from 0 to -100
                        break;

                    case OrderType.CloseShort:
                        // decrease short (less negative)
                        position.Quantity += order.Quantity;  // e.g. from -100 to -70
                        break;
                }

                // 5) If fully closed, remove; otherwise update cost/current price
                if (position.Quantity == 0)
                {
                    positions.Remove(order.Symbol);
                }
                else
                {
                    position.CurrentPrice = order.Price;
                    // AveragePurchasePrice was only recalculated in the two “adding” cases
                }
            }

            // 6) Persist
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

        
    }
}
