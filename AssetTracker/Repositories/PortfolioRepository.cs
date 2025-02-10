using System;
using AssetTracker.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace AssetTracker.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly Dictionary<Guid, Portfolio> _userPortfolios;
        private readonly IDistributedCache _cache;

        public PortfolioRepository()
        {
            _userPortfolios = new(); // Initialize with an empty list (replace with actual DB logic)

        }


        public async Task<Portfolio> GetUserPortfolioAsync(Guid userId)
        {
            await Task.Delay(10); // Simulated delay for async operation

            if (_userPortfolios.ContainsKey(userId))
            {
                return _userPortfolios[userId];

            }
            else
            {
                throw new InvalidOperationException("Portfolio not found.");

            }


        }
        public async Task UpdatePortfolioAsync(Portfolio portfolio)
        {
            // Simulating an async call to update the portfolio (you can replace this with actual async DB operations)
            await Task.Delay(10); // Simulated delay for async operation

            if (_userPortfolios.ContainsKey(portfolio.UserId))
            {
                _userPortfolios[portfolio.UserId] = portfolio;
            }
            else
            {
                // If the portfolio does not exist in the dictionary, add it.
                _userPortfolios.Add(portfolio.UserId, portfolio);
            }
        }
        public async Task<Dictionary<string, Position>> GetPositionsByUserId(Guid userId)
        {
            if (!_userPortfolios.ContainsKey(userId))
            {
                throw new InvalidOperationException("Portfolio not found.");

            }
            return _userPortfolios[userId].Positions;


        }
        public async Task AddPortfolioAsync(Portfolio portfolio)
        {
            if (portfolio == null) throw new ArgumentNullException(nameof(portfolio));

            _userPortfolios[portfolio.UserId] = portfolio;

            await Task.CompletedTask; // Simulate async behavior, but in memory it's instantaneous
        }

        public async Task<Position> GetUserPositionBySymbol(Guid userId, string symbol)
        {
            if (!_userPortfolios.ContainsKey(userId))
            {
                throw new InvalidOperationException("Portfolio not found.");

            }
            try {
                var position = _userPortfolios[userId].Positions[symbol];
                return position;

            }
            catch (Exception)
            {
                return null;
            }
        }

        //public  async Task AddPortfolioAsync(Portfolio portfolio)
        //{
        //    _userPortfolios.Add(portfolio); // Add portfolio to the in-memory list (replace with DB save logic)
        //    await Task.CompletedTask; // Simulate async task
        //}

        //public async Task AddPositionToPortfolioAsync( Position position, Guid userId)
        //{

        //    var portfolio = _userPortfolios.FirstOrDefault(p => p.UserId == userId);
        //    if (portfolio == null)
        //    {
        //        throw new InvalidOperationException("Portfolio not found.");
        //    }

        //    portfolio.Positions.Add(position); // Just add the position to the portfolio
        //    await Task.CompletedTask; // Simulate async task
        //}

        //public async Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync()
        //{
        //    return await Task.FromResult(_portfolios); // Simulate async behavior
        //}





        //public async Task RemovePortfolioAsync(Guid userId)
        //{
        //    var portfolio = _portfolios.FirstOrDefault(p => p.UserId == userId);
        //    if (portfolio != null)
        //    {
        //        _portfolios.Remove(portfolio); // Remove portfolio from the in-memory list
        //    }
        //    await Task.CompletedTask; // Simulate async task
        //}

        //public async Task RemovePositionFromPortfolioAsync(Guid portfolioId, string symbol)
        //{
        //    var portfolio = _portfolios.FirstOrDefault(p => p.PortfolioId == portfolioId);
        //    if (portfolio != null)
        //    {
        //        var position = portfolio.Positions.FirstOrDefault(p => p.StockSymbol == symbol);
        //        if (position != null)
        //        {
        //            portfolio.Positions.Remove(position); // Remove the position
        //        }
        //    }
        //    await Task.CompletedTask; // Simulate async task
        //}

        //public async Task UpdatePortfolioAsync(Portfolio portfolio)
        //{
        //    var existingPortfolio = _portfolios.FirstOrDefault(p => p.UserId == portfolio.UserId);
        //    if (existingPortfolio != null)
        //    {
        //        // Update the portfolio's properties (you could add more business logic here)
        //        //existingPortfolio.Name = portfolio.Name;
        //        existingPortfolio.Positions = portfolio.Positions;
        //    }
        //    await Task.CompletedTask; // Simulate async task
        //}
    }
}

