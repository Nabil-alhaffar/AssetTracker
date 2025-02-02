﻿using System;
using AssetTracker.Models;

namespace AssetTracker.Repositories
{
	public class PortfolioRepository:IPortfolioRepository
	{
        private readonly List<Portfolio> _portfolios; // Simulating an in-memory store of portfolios

        public PortfolioRepository()
        {
            _portfolios = new List<Portfolio>(); // Initialize with an empty list (replace with actual DB logic)

        }

        public  async Task AddPortfolioAsync(Portfolio portfolio)
        {
            _portfolios.Add(portfolio); // Add portfolio to the in-memory list (replace with DB save logic)
            await Task.CompletedTask; // Simulate async task
        }

        public async Task AddPositionToPortfolioAsync(int portfolioId, Position position)
        {
            var portfolio = _portfolios.FirstOrDefault(p => p.Id == portfolioId);
            if (portfolio != null)
            {
                portfolio.Positions.Add(position); // Add the position to the portfolio
            }
            await Task.CompletedTask; // Simulate async task
        }

        public async Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync()
        {
            return await Task.FromResult(_portfolios); // Simulate async behavior
        }

        public async Task<Portfolio> GetUserPortfolioAsync(int userId)
        {
            var portfolio = _portfolios.FirstOrDefault(p => p.Id == userId);
            return await Task.FromResult(portfolio); // Simulate async behavior

        }

        public async Task RemovePortfolioAsync(int portfolioId)
        {
            var portfolio = _portfolios.FirstOrDefault(p => p.Id == portfolioId);
            if (portfolio != null)
            {
                _portfolios.Remove(portfolio); // Remove portfolio from the in-memory list
            }
            await Task.CompletedTask; // Simulate async task
        }

        public async Task RemovePositionFromPortfolioAsync(int portfolioId, string symbol)
        {
            var portfolio = _portfolios.FirstOrDefault(p => p.Id == portfolioId);
            if (portfolio != null)
            {
                var position = portfolio.Positions.FirstOrDefault(p => p.StockSymbol == symbol);
                if (position != null)
                {
                    portfolio.Positions.Remove(position); // Remove the position
                }
            }
            await Task.CompletedTask; // Simulate async task
        }

        public async Task UpdatePortfolioAsync(Portfolio portfolio)
        {
            var existingPortfolio = _portfolios.FirstOrDefault(p => p.Id == portfolio.Id);
            if (existingPortfolio != null)
            {
                // Update the portfolio's properties (you could add more business logic here)
                existingPortfolio.Owner = portfolio.Owner;
                existingPortfolio.Positions = portfolio.Positions;
            }
            await Task.CompletedTask; // Simulate async task
        }
    }
}

