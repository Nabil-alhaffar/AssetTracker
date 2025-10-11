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
using AssetTracker.Models.Enums;
namespace AssetTracker.Services
{

    /// <summary>
    /// Service for managing user positions in a portfolio, including updates, summaries, and history tracking.
    /// </summary>
    public class PositionService : IPositionService
    {
        private readonly ILogger<PositionService> _logger;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IAlphaVantageStockMarketService _alphaVantageStockMarketService; // Added to get current stock price
        private readonly Dictionary<Guid, List<PositionHistory>> _positionHistoryStorage = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionService"/> class.
        /// </summary>
        /// <param name="logger">Logger for logging information.</param>
        /// <param name="portfolioRepository">Repository for accessing portfolio data.</param>
        /// <param name="alphaVantageStockMarketService">Service for fetching stock market data.</param>
        public PositionService(ILogger<PositionService> logger, IPortfolioRepository portfolioRepository, IAlphaVantageStockMarketService alphaVantageStockMarketService)
        {
            _portfolioRepository = portfolioRepository;
            _alphaVantageStockMarketService = alphaVantageStockMarketService;
            _logger = logger;
        }

        /// <summary>
        /// Splits a position in the user's portfolio based on a split factor.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <param name="symbol">Stock symbol.</param>
        /// <param name="splitFactor">Factor to split the position by.</param>
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

        /// <summary>
        /// Checks if a position has triggered a stop-loss condition.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <param name="symbol">Stock symbol.</param>
        /// <param name="stopLossPrice">Price threshold for stop loss.</param>
        /// <returns>True if stop loss is triggered, otherwise false.</returns>
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

        /// <summary>
        /// Updates the profit and loss of a position based on the current price.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <param name="symbol">Stock symbol.</param>
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


        /// <summary>
        /// Adds a position transaction history record for a user.
        /// </summary>
        /// <param name="history">Position history record.</param>
        public async Task AddPositionHistoryAsync(PositionHistory history)
        {
            if (!_positionHistoryStorage.ContainsKey(history.UserId))
            {
                _positionHistoryStorage[history.UserId] = new List<PositionHistory>();
            }

             _positionHistoryStorage[history.UserId].Add(history);
        }

        /// <summary>
        /// Retrieves the position history for a user, optionally filtered by symbol.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <param name="symbol">Optional stock symbol to filter by.</param>
        /// <returns>List of position history records.</returns>
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


        /// <summary>
        /// Gets a summary of a specific position in the user's portfolio.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <param name="symbol">Stock symbol.</param>
        /// <returns>Position summary object or null if position is not found.</returns>
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

        /// <summary>
        /// Gets a summary of a specific position in the user's portfolio.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <param name="symbol">Stock symbol.</param>
        /// <returns>Position summary object or null if position is not found.</returns>
        private async Task<PnL> GetPositionPnL (Position position)
        {
            decimal openPNL = 0;

            decimal currentPrice = position.CurrentPrice; // Fetch current market price for the symbol
            decimal averageEntryPrice = position.AveragePurchasePrice;
            decimal quantity = position.Quantity;

            if (position.Type == PositionType.Long) // Long position
            {
                openPNL += (currentPrice - averageEntryPrice) * quantity;
            }
            else if (position.Type == PositionType.Short) // Short position
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


        /// <summary>
        /// Retrieves a specific position by symbol from the user's portfolio.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <param name="symbol">Stock symbol.</param>
        /// <returns>The position object, or null if not found.</returns>
        public async Task<Position> GetPositionAsync(Guid userId, string symbol)
        {
            return await _portfolioRepository.GetUserPositionBySymbol(userId, symbol);

        }


        /// <summary>
        /// Updates a user's portfolio based on the given filled order.
        /// </summary>
        /// <param name="order">Order that affects the position.</param>
        /// <param name="fundChanges">Optional fund changes to apply (AvailableFunds and MarginUsed deltas).</param>
        public async Task UpdatePositionAsync(Order order, (decimal availableFundsDelta, decimal marginUsedDelta)? fundChanges = null)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(order.UserId);
            if (portfolio == null)
                throw new KeyNotFoundException($"No portfolio found for user {order.UserId}");

            // Apply fund changes if provided
            if (fundChanges.HasValue)
            {
                portfolio.AvailableFunds += fundChanges.Value.availableFundsDelta;
                portfolio.MarginUsed += fundChanges.Value.marginUsedDelta;
                
                // Ensure margin used is never negative
                if (portfolio.MarginUsed < 0)
                {
                    portfolio.MarginUsed = 0;
                }
            }

            var positions = portfolio.Positions;

            if (!positions.TryGetValue(order.Symbol, out var position))
            {
                // Only open intents should create new positions
                if (order.Intent == TradeIntent.BuyToClose || order.Intent == TradeIntent.SellToClose)
                    throw new InvalidOperationException($"Cannot {order.Intent} without an existing position.");

                var quantity = order.Intent == TradeIntent.SellToOpen ? -order.Quantity : order.Quantity;
                position = new Position
                {
                    UserId = order.UserId,
                    Symbol = order.Symbol,
                    Quantity = quantity,
                    AveragePurchasePrice = order.Price,
                    CurrentPrice = order.Price,
                    Type = quantity < 0 ? PositionType.Short : PositionType.Long
                };
                positions[order.Symbol] = position;
            }
            else
            {
                // Enforce position rules
                switch (order.Intent)
                {
                    case TradeIntent.BuyToOpen:
                        if (position.Type == PositionType.Short)
                            throw new InvalidOperationException("Must close short before opening long.");
                        position.AveragePurchasePrice =
                            ((position.AveragePurchasePrice * position.Quantity) + (order.Price * order.Quantity)) /
                            (position.Quantity + order.Quantity);
                        position.Quantity += order.Quantity;
                        break;

                    case TradeIntent.BuyToClose:
                        if (position.Type != PositionType.Short)
                            throw new InvalidOperationException("No short position to close.");
                        if (-position.Quantity < order.Quantity)
                            throw new InvalidOperationException("Trying to buy more than shorted.");
                        position.Quantity += order.Quantity;
                        break;

                    case TradeIntent.SellToOpen:
                        if (position.Type == PositionType.Long)
                            throw new InvalidOperationException("Must sell long before opening short.");
                        position.AveragePurchasePrice =
                            ((Math.Abs(position.Quantity) * position.AveragePurchasePrice) + (order.Price * order.Quantity)) /
                            (Math.Abs(position.Quantity) + order.Quantity);
                        position.Quantity -= order.Quantity;
                        break;

                    case TradeIntent.SellToClose:
                        if (position.Type != PositionType.Long)
                            throw new InvalidOperationException("No long position to close.");
                        if (position.Quantity < order.Quantity)
                            throw new InvalidOperationException("Trying to sell more than owned.");
                        position.Quantity -= order.Quantity;
                        break;
                }

                // Cleanup if position is fully closed
                if (position.Quantity == 0)
                {
                    positions.Remove(order.Symbol);
                }
                else
                {
                    position.CurrentPrice = order.Price;
                }
            }

            await _portfolioRepository.UpdatePortfolioAsync(portfolio);

            string actionType = order.Intent switch
            {
                TradeIntent.BuyToOpen => "BUY",
                TradeIntent.SellToClose => "SELL",
                TradeIntent.SellToOpen => "SELL SHORT",
                TradeIntent.BuyToClose => "BUY TO COVER",
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
