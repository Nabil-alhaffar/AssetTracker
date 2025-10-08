using System;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using AssetTracker.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.Extensions.Caching.Distributed;
using AssetTracker.Repositories;
using AssetTracker.Services.Interfaces;
using AssetTracker.Repositories.Interfaces;
using AssetTracker.Repositories.MongoDBRepositories;
using AssetTracker.Models.Enums;

namespace AssetTracker.Services
{
    /// <summary>
    /// Provides stock trading operations.
    /// </summary>
    public class StockService : IStockService
    {
        private readonly IPositionService _positionService;
        private readonly IAlphaVantageStockMarketService _alphaVantageStockMarketService;
        private readonly IPortfolioService _portfolioService;
        private readonly IOrderRepository _orderRepository;



        public StockService(  IAlphaVantageStockMarketService stockMarketService, IPortfolioService portfolioService, IPositionService positionService, IOrderRepository orderRepository )
        {
            _alphaVantageStockMarketService = stockMarketService;
            _portfolioService = portfolioService;
            _positionService = positionService;
            _orderRepository = orderRepository;

        }
        /// <summary>
        /// Executes a trade request for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user executing the trade.</param>
        /// <param name="tradeRequest">The trade request details.</param>
        /// <returns>A task that returns a <see cref="TradeResponse"/> indicating the result of the trade execution.</returns>
        public async Task<TradeResponse> ExecuteTradeAsync(Guid userId, TradeRequest tradeRequest)
        {
            if (tradeRequest.Quantity <= 0)
                return new TradeResponse(false, "Quantity must be greater than 0");

            var price = await _alphaVantageStockMarketService.GetStockPriceAsync(tradeRequest.Symbol);
            var totalValue = tradeRequest.Quantity * price;

            var portfolio = await _portfolioService.GetPortfolioAsync(userId);
            if (portfolio == null)
                return new TradeResponse(false, "Portfolio not found.");

            var position = await _positionService.GetPositionAsync(userId, tradeRequest.Symbol);
            var intent = tradeRequest.Intent;

            if (!IsSideIntentCombinationValid(tradeRequest.Side, intent))
                return new TradeResponse(false, $"Inconsistent side ({tradeRequest.Side}) and intent ({intent}) combination.");

            // Check for margin call - only allow closing positions when in margin call
            if (portfolio.IsInMarginCall && (intent == TradeIntent.BuyToOpen || intent == TradeIntent.SellToOpen))
                return new TradeResponse(false, "Cannot open new positions while in margin call. Please close existing positions or add funds.");

            decimal marginRequired = totalValue * portfolio.InitialMarginRequirement;

            // Validate trade conditions
            switch (intent)
            {
                case TradeIntent.BuyToOpen:
                    if (position?.Type == PositionType.Short)
                        return new TradeResponse(false, "Close your short position before opening a long position.");

                    if (portfolio.BuyingPower < totalValue)
                        return new TradeResponse(false, "Insufficient buying power.");
                    break;

                case TradeIntent.BuyToClose:
                    if (position == null || position.Type != PositionType.Short)
                        return new TradeResponse(false, "No short position to close.");
                    if (-position.Quantity < tradeRequest.Quantity)
                        return new TradeResponse(false, "Trying to close more than shorted.");
                    break;

                case TradeIntent.SellToOpen:
                    if (position?.Type == PositionType.Long)
                        return new TradeResponse(false, "Close your long position before opening a short position.");

                    if (portfolio.BuyingPower < marginRequired)
                        return new TradeResponse(false, "Insufficient buying power to short.");
                    break;

                case TradeIntent.SellToClose:
                    if (position == null || position.Type != PositionType.Long)
                        return new TradeResponse(false, "No long position to sell.");
                    if (position.Quantity < tradeRequest.Quantity)
                        return new TradeResponse(false, "Not enough shares to sell.");
                    break;

                default:
                    return new TradeResponse(false, "Unknown trade intent.");
            }

            var order = new Order
            {
                UserId = userId,
                Symbol = tradeRequest.Symbol,
                Quantity = tradeRequest.Quantity,
                Price = price,
                Side = tradeRequest.Side,
                Intent = tradeRequest.Intent,
                Timestamp = DateTime.UtcNow
            };

            // Calculate fund changes
            (decimal availableFundsDelta, decimal marginUsedDelta) fundChanges = (0, 0);
            switch (intent)
            {
                case TradeIntent.BuyToOpen:
                    fundChanges = (-totalValue, 0);
                    break;
                case TradeIntent.BuyToClose:
                    fundChanges = (-totalValue, -marginRequired);
                    break;
                case TradeIntent.SellToOpen:
                    fundChanges = (totalValue, marginRequired);
                    break;
                case TradeIntent.SellToClose:
                    fundChanges = (totalValue, 0);
                    break;
            }

            await _positionService.UpdatePositionAsync(order, fundChanges);
            await _orderRepository.AddOrderAsync(order);

            return new TradeResponse(true, $"{intent} {tradeRequest.Quantity} shares of {tradeRequest.Symbol} at ${price}.");
        }



        /// <summary>
        /// Checks if a trade request is valid or contradictory in the context of intent and side.
        /// </summary>
        /// <param name="side">The trade request side</param>
        /// <param name="intent">The trade request intent.</param>
        /// <returns> True if the combination is valid, otherwisefalse</returns>
        private static bool IsSideIntentCombinationValid(TradeSide side, TradeIntent intent)
        {
            return (side == TradeSide.Buy && (intent == TradeIntent.BuyToOpen || intent == TradeIntent.BuyToClose)) ||
                   (side == TradeSide.Sell && (intent == TradeIntent.SellToOpen || intent == TradeIntent.SellToClose));
        }


    }
}

