using System;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services
{
    public class PositionService : IPositionService
    {
        private readonly StockService _stockService;
        private static readonly List<Position> _positions = new List<Position>();

        private readonly IPortfolioRepository _portfolioRepository;


        public PositionService()
        {
            //_positions = new List<Position>();
        }

        public async Task UpdatePositionAsync(int userId, string symbol, double additionalQuantity, double purchasePrice)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Stock.Symbol == symbol);

            if (position != null)
            {
                // Update the quantity of the position
                double newQuantity = position.Quantity + additionalQuantity;

                // Update the average purchase price based on the weighted average formula
                double totalCost = (position.Quantity * position.AveragePurchasePrice) + (additionalQuantity * purchasePrice);
                double newAveragePurchasePrice = totalCost / newQuantity;

                // Set the updated values
                position.Quantity = newQuantity;
                position.AveragePurchasePrice = newAveragePurchasePrice;

                //await _portfolioRepository.SaveChangesAsync();  // Save changes to the database
            }
        }

        public async Task SplitPositionAsync(int userId, string symbol, int splitFactor)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Stock.Symbol == symbol);

            if (position != null)
            {
                // For a 1:2 split, for example, the number of shares doubles.
                position.Quantity *= splitFactor;
                position.AveragePurchasePrice /= splitFactor;  // Adjust the average price accordingly

                //await _portfolioRepository.SaveChangesAsync();  // Save changes to the database
            }
        }
        public async Task CheckPositionForStopLossAsync(int userId, string symbol, double stopLossPrice)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Stock.Symbol == symbol);

            if (position != null)
            {
                Stock stock = await _stockService.GetStockOverviewAsync(symbol);
                double currentPrice = stock.CurrentPrice;
                if (currentPrice <= stopLossPrice)
                {
                    // Stop-loss triggered, take action
                    Console.WriteLine($"Stop-loss triggered for {symbol}. Consider selling the position.");
                    // Send notification, email, or trigger a sale
                }
            }
        }
        public async Task UpdatePositionProfitLossAsync(int userId, string symbol)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Stock.Symbol == symbol);

            if (position != null)
            {
                var stock = await _stockService.GetStockOverviewAsync(symbol);
                double currentPrice = stock.CurrentPrice;
                double currentProfitLoss = (currentPrice - position.AveragePurchasePrice) * position.Quantity;

                position.PNL = currentProfitLoss;

                //await _portfolioRepository.SaveChangesAsync();  // Save changes to the database
            }
        }
        // Create a new position by fetching stock data from Alpha Vantage

        //public async Task AddPositionAsync(Position position)
        //{
        //    _positions.Add(position);
        //    await Task.CompletedTask;
        //}

        //public async Task RemovePositionAsync(string stockSymbol)
        //{
        //    var position = _positions.FirstOrDefault(p => p.StockSymbol == stockSymbol);
        //    if (position != null)
        //    {
        //        _positions.Remove(position);
        //    }
        //    await Task.CompletedTask;
        //}

        //public async Task<Position> GetPositionAsync(string stockSymbol)
        //{
        //    var position = _positions.FirstOrDefault(p => p.StockSymbol == stockSymbol);
        //    return await Task.FromResult(position);
        //}
        //public async Task<List<Position>> GetAllPositionsAsync()
        //{
        //    return await Task.FromResult(_positions);
        //}



    }
}

