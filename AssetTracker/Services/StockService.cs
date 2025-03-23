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

namespace AssetTracker.Services
{
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

        public async Task<TradeResult> ExecuteTradeAsync(Guid userId, TradeRequest tradeRequest)
        {
            if (tradeRequest.Quantity <= 0)
                return new TradeResult(false, "Quantity must be greater than 0");

            var price = await _alphaVantageStockMarketService.GetStockPriceAsync(tradeRequest.Symbol);
            var totalValue = tradeRequest.Quantity * price;
            var availableFunds = await _portfolioService.GetAvailableFundsAsync(userId);
            var position = await _positionService.GetPositionAsync(userId, tradeRequest.Symbol);

            if (tradeRequest.Type == OrderType.Buy && position != null && position.Type == Position.PositionType.Short)
                return new TradeResult(false, "Close your short position before buying long.");

            if (tradeRequest.Type == OrderType.Short && position != null && position.Type == Position.PositionType.Long)
                return new TradeResult(false, "Sell your long position before shorting.");

            switch (tradeRequest.Type)
            {
                case OrderType.Buy:
                    if (totalValue > availableFunds)
                        return new TradeResult(false, "Insufficient funds.");
                    await _portfolioService.UpdateAvailableFundsAsync(userId, -totalValue);
                    break;

                case OrderType.Sell:
                    if (position == null || position.Quantity < tradeRequest.Quantity)
                        return new TradeResult(false, "Not enough shares to sell.");
                    await _portfolioService.UpdateAvailableFundsAsync(userId, totalValue);
                    break;

                case OrderType.Short:
                    await _portfolioService.UpdateAvailableFundsAsync(userId, totalValue);
                    break;

                case OrderType.CloseShort:
                    if (position == null || position.Quantity >= 0)
                        return new TradeResult(false, "No short positions to close.");
                    if (-position.Quantity < tradeRequest.Quantity)
                        return new TradeResult(false, "Cannot buy back more shares than were shorted.");
                    await _portfolioService.UpdateAvailableFundsAsync(userId, -totalValue);
                    break;

                default:
                    return new TradeResult(false, "Invalid trade type.");
            }

            // ✅ Create an `Order` object
            var order = new Order
            {
                UserId = userId,
                Symbol = tradeRequest.Symbol,
                Quantity = tradeRequest.Quantity,
                Price = price,
                Type = tradeRequest.Type,
                Timestamp = DateTime.UtcNow
            };

            await _positionService.UpdatePositionAsync(order);
            await _orderRepository.AddOrderAsync(order);

            return new TradeResult(true, $"{tradeRequest.Type} {tradeRequest.Quantity} shares of {tradeRequest.Symbol} at ${price}.");
        }
    }
}

