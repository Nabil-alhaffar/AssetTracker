using System;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services
{
    public class PositionService : IPositionService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly Dictionary<int, List<PositionHistory>> _positionHistoryStorage = new();


        // Constructor
        public PositionService(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        // Update a position (adding more quantity and adjusting purchase price)
        public async Task UpdatePositionAsync(Position updatedPosition, int userId)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.StockSymbol == updatedPosition.StockSymbol);

            if (position != null)
            {
                position.Quantity = updatedPosition.Quantity;
                position.AveragePurchasePrice = updatedPosition.AveragePurchasePrice;
            }
            else
            {
                portfolio.Positions.Add(updatedPosition);
            }

            await _portfolioRepository.UpdatePortfolioAsync(portfolio);
        }

        // Split a position based on the split factor
        public async Task SplitPositionAsync(int userId, string symbol, int splitFactor)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Stock.Symbol == symbol);

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
        public async Task<bool> CheckPositionForStopLossAsync(int userId, string symbol, decimal stopLossPrice)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Stock.Symbol == symbol) ?? throw new InvalidOperationException("Position not found.");

            // Compare the current market price of the stock with the stop-loss price
            if (position.Stock.CurrentPrice <= stopLossPrice)
            {
                return true;  // Stop loss triggered
            }

            return false;  // Stop loss not triggered
        }

        // Update Profit & Loss for a given position
        public async Task UpdatePositionProfitLossAsync(int userId, string symbol)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            var position = portfolio.Positions.FirstOrDefault(p => p.Stock.Symbol == symbol);

            if (position == null)
                throw new InvalidOperationException("Position not found.");

            // Calculate profit/loss based on the current price and purchase price
            decimal? marketValue = position.Stock.CurrentPrice * position.Quantity;
            decimal totalCost = position.AveragePurchasePrice * position.Quantity;

            // Assuming PNL is a field in the Position model, calculate and update it
            //position.PNL = marketValue - totalCost;

            // Persist the changes back to the repository
            await _portfolioRepository.UpdatePortfolioAsync(portfolio);  // Save updated portfolio
        }

        // Add a new position to the portfolio
        public async Task AddPositionAsync(Position position, int userId)
        {
            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            if (portfolio != null)
            {
                // Check if the position already exists in the portfolio
                var existingPosition = portfolio.Positions.FirstOrDefault(p => p.Stock.Symbol == position.Stock.Symbol);
                if (existingPosition != null)
                {
                    // Update the position if it exists (e.g., adding quantity)
                    await UpdatePositionAsync(position, userId);
                }
                else
                {
                    // Add the new position to the portfolio
                    portfolio.Positions.Add(position);

                    // Persist the changes back to the repository
                    await _portfolioRepository.UpdatePortfolioAsync(portfolio);  // Save updated portfolio
                }
            }
        }

        public async Task<List<PositionHistory>> GetPositionHistoryAsync(int userId, string symbol)
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

        public async Task AddPositionHistoryAsync(PositionHistory history)
        {
            if (!_positionHistoryStorage.ContainsKey(history.UserId))
            {
                _positionHistoryStorage[history.UserId] = new List<PositionHistory>();
            }
            _positionHistoryStorage[history.UserId].Add(history);
        }

        // Optionally, you could have a method to remove a position as well
        // public async Task RemovePositionAsync(int userId, string symbol)
        // {
        //     var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
        //     var position = portfolio.Positions.FirstOrDefault(p => p.Stock.Symbol == symbol);
        //     if (position != null)
        //     {
        //         portfolio.Positions.Remove(position);
        //         await _portfolioRepository.UpdatePortfolioAsync(portfolio);  // Save changes
        //     }
        // }
    }
}
