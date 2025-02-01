using System;
using AssetTracker.Models;

namespace AssetTracker.Repositories
{
	public interface IPortfolioRepository
	{
        Task<Portfolio> GetPortfolioAsync(int portfolioId);
        Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync();
        Task AddPortfolioAsync(Portfolio portfolio);
        Task UpdatePortfolioAsync(Portfolio portfolio);
        Task RemovePortfolioAsync(int portfolioId);
        Task AddPositionToPortfolioAsync(int portfolioId, Position position);
        Task RemovePositionFromPortfolioAsync(int portfolioId, string symbol);

    }
}

