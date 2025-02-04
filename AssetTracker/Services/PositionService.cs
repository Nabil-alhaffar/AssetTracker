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
        public async Task<bool> CheckPositionForStopLossAsync(int userId, string symbol, double stopLossPrice)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Stock.Symbol == symbol) ?? throw new InvalidOperationException("Position not found.");

            // Compare the current market price of the stock with the stop loss price
            if (position.Stock.CurrentPrice <= stopLossPrice)
            {
                return true;  // Stop loss triggered
            }

            return false;  // Stop loss not triggered
        }
        public async Task UpdatePositionProfitLossAsync(int userId, string symbol)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Stock.Symbol == symbol);

            if (position == null)
                throw new InvalidOperationException("Position not found.");

            // Calculate profit/loss based on the current price and purchase price
            double? marketValue = position.Stock.CurrentPrice * position.Quantity;
            double totalCost = position.AveragePurchasePrice * position.Quantity;

            //position.PNL = (double)(marketValue - totalCost);

            //await _portfolioRepository.SaveChangesAsync();  // Commit changes
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

