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
using static AssetTracker.Models.Position;

namespace AssetTracker.Services
{
    public class StockService : IStockService
    {
        private readonly IPositionService _positionService;
        private readonly IAlphaVantageStockMarketService _alphaVantageStockMarketService;
        private readonly IPortfolioService _portfolioService;



        public StockService(IAlphaVantageStockMarketService stockMarketService, IPortfolioService portfolioService, IPositionService positionService)
        {
            _alphaVantageStockMarketService = stockMarketService;
            _portfolioService = portfolioService;
            _positionService = positionService;

        }

        public async Task<TradeResult> ExecuteTradeAsync(Guid userId, TradeRequest tradeRequest)
        {
            if (tradeRequest.Quantity <= 0)
                return new TradeResult(false, "Quantity must be greater than 0");

            var price = await _alphaVantageStockMarketService.GetStockPriceAsync(tradeRequest.Symbol);
            var totalValue = tradeRequest.Quantity * price;
            var availableFunds = await _portfolioService.GetAvailableFundsAsync(userId);
            var position = await _positionService.GetPositionAsync(userId, tradeRequest.Symbol);

            // 🚨 Validate trade restrictions
            if (tradeRequest.Type == OrderType.Buy && position != null && position.Type == PositionType.Short)
                return new TradeResult(false, "Close your short position before buying long.");

            if (tradeRequest.Type == OrderType.Short && position != null && position.Type == PositionType.Long)
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
            //await _orderRepository.SaveOrderAsync(order);

            return new TradeResult(true, $"{tradeRequest.Type} {tradeRequest.Quantity} shares of {tradeRequest.Symbol} at ${price}.");
        }




        //public async Task<TradeResult> BuyStockAsync (Guid userId, string symbol , decimal quantity)
        //{
        //    if (quantity <= 0)
        //        return new TradeResult(false, "Quantity must be greater than 0");
        //    Console.WriteLine($"Buying stock: userId={userId}, symbol={symbol}, quantity={quantity}");

        //    var price = await _alphaVantageStockMarketService.GetStockPriceAsync(symbol);
        //    Console.WriteLine($"price obtained successfully = {price}");
        //    var totalCost = quantity * price;
        //    var availableFunds = await _portfolioService.GetAvailableFundsAsync(userId);
        //    Console.WriteLine($"Availablefunds obtained successfully = {availableFunds}");

        //    if (totalCost > availableFunds)
        //        return new TradeResult(false, "Insufficient funds.");
        //    await _portfolioService.UpdateAvailableFundsAsync(userId, -totalCost);
        //    Console.WriteLine("Updated portfolio funds");

        //    await _positionService.AddOrUpdatePositionAsync(userId, symbol, quantity, price);
        //    Console.WriteLine("Updated portfolio positions");

        //    return new TradeResult(true, $"Bought {quantity} shares of {symbol} at ${price}");

        //}

        //public async Task <TradeResult> SellStockAsync(Guid userId, string symbol, decimal quantity)
        //{
        //    if (quantity <= 0)
        //    {
        //        return new TradeResult(false, "Quantity must be greater than 0");
        //    }
        //    var position = await _positionService.GetPositionAsync(userId, symbol);
        //    if(position==null|| position.Quantity<quantity)
        //        return new TradeResult(false, "Not enough shares to sell");
        //    var price = await _alphaVantageStockMarketService.GetStockPriceAsync(symbol);
        //    var totalSale = price * quantity;
        //    await _portfolioService.UpdateAvailableFundsAsync(userId, totalSale);
        //    await _positionService.ReduceOrRemovePositionAsync(userId, symbol, quantity,price);
        //    return new TradeResult(true, $"Sold {quantity} shares of {symbol} at ${price}");


        //}

        //public async Task <TradeResult> ShortStockAsync(Guid userId, string symbol, decimal quantity)
        //{
        //    var price = await _alphaVantageStockMarketService.GetStockPriceAsync(symbol);
        //    var totalProceeds = price * quantity;

        //    await _portfolioService.UpdateAvailableFundsAsync(userId, totalProceeds);
        //    await _positionService.AddOrUpdateShortPositionAsync(userId, symbol, quantity, price);
        //    return new TradeResult(true, $"Shorted {quantity} shares of {symbol} at ${price}");

        //}

        //public async Task<TradeResult> CloseShortAsync (Guid userId, string symbol, decimal quantity)
        //{
        //    var position = await _positionService.GetPositionAsync(userId, symbol);
        //    if (position == null || position.Quantity >= 0)
        //        return new TradeResult(false, "No short positions to close");

        //    if (-position.Quantity < quantity)
        //        return new TradeResult(false, "Cannot buy back more shares than were shorted.");
        //    var price = await _alphaVantageStockMarketService.GetStockPriceAsync(symbol);
        //    var totalCost = price * quantity;

        //    await _portfolioService.UpdateAvailableFundsAsync(userId, -totalCost);
        //    await _positionService.ReduceOrRemovePositionAsync(userId, symbol, -quantity,price);

        //    return new TradeResult(true, $"Closed Short: Bought back {quantity} shares of {symbol} at ${price}.");



        //}




    }


}

