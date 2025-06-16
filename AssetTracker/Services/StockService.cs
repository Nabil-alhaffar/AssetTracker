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

        public async Task<TradeResponse> ExecuteTradeAsync(Guid userId, TradeRequest tradeRequest)
        {
            if (tradeRequest.Quantity <= 0)
                return new TradeResponse(false, "Quantity must be greater than 0");

            var price = await _alphaVantageStockMarketService.GetStockPriceAsync(tradeRequest.Symbol);
            var totalValue = tradeRequest.Quantity * price;
            var availableFunds = await _portfolioService.GetAvailableFundsAsync(userId);
            var position = await _positionService.GetPositionAsync(userId, tradeRequest.Symbol);

            if (tradeRequest.Type == OrderSide.Buy && position != null && position.Type == PositionType.Short)
                return new TradeResponse(false, "Close your short position before buying long.");

            if (tradeRequest.Type == OrderSide.Short && position != null && position.Type == PositionType.Long)
                return new TradeResponse(false, "Sell your long position before shorting.");

            switch (tradeRequest.Type)
            {
                case OrderSide.Buy:
                    if (totalValue > availableFunds)
                        return new TradeResponse(false, "Insufficient funds.");
                    await _portfolioService.UpdateAvailableFundsAsync(userId, -totalValue);
                    break;

                case OrderSide.Sell:
                    if (position == null || position.Quantity < tradeRequest.Quantity)
                        return new TradeResponse(false, "Not enough shares to sell.");
                    await _portfolioService.UpdateAvailableFundsAsync(userId, totalValue);
                    break;

                case OrderSide.Short:
                    await _portfolioService.UpdateAvailableFundsAsync(userId, totalValue);
                    break;

                case OrderSide.CloseShort:
                    if (position == null || position.Quantity >= 0)
                        return new TradeResponse(false, "No short positions to close.");
                    if (-position.Quantity < tradeRequest.Quantity)
                        return new TradeResponse(false, "Cannot buy back more shares than were shorted.");
                    await _portfolioService.UpdateAvailableFundsAsync(userId, -totalValue);
                    break;

                default:
                    return new TradeResponse(false, "Invalid trade type.");
            }

            // ✅ Create an `Order` object
            var order = new Order
            {
                UserId = userId,
                Symbol = tradeRequest.Symbol,
                Quantity = tradeRequest.Quantity,
                Price = price,
                Side = tradeRequest.Type,
                Timestamp = DateTime.UtcNow
            };

            await _positionService.UpdatePositionAsync(order);
            await _orderRepository.AddOrderAsync(order);

            return new TradeResponse(true, $"{tradeRequest.Type} {tradeRequest.Quantity} shares of {tradeRequest.Symbol} at ${price}.");
        }
    }
}

