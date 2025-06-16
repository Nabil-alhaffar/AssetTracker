using System;
using AssetTracker.Models;

namespace AssetTracker.Repositories.Interfaces
{
    /// <summary>
    /// Interface for portfolio data persistence and retrieval operations.
    /// </summary>
    public interface IPortfolioRepository
    {
        /// <summary>
        /// Retrieves the portfolio for a specified user asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>The user's portfolio.</returns>
        Task<Portfolio> GetUserPortfolioAsync(Guid userId);

        /// <summary>
        /// Updates an existing portfolio asynchronously.
        /// </summary>
        /// <param name="portfolio">The portfolio to update.</param>
        Task UpdatePortfolioAsync(Portfolio portfolio);

        /// <summary>
        /// Retrieves all positions in the user's portfolio asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A dictionary of positions keyed by stock symbol.</returns>
        Task<Dictionary<string, Position>> GetPositionsByUserId(Guid userId);

        /// <summary>
        /// Retrieves a specific position by symbol for a given user asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The matching position.</returns>
        Task<Position> GetUserPositionBySymbol(Guid userId, string symbol);

        /// <summary>
        /// Adds a new portfolio asynchronously.
        /// </summary>
        /// <param name="portfolio">The portfolio to add.</param>
        Task AddPortfolioAsync(Portfolio portfolio);

        /// <summary>
        /// Retrieves all user IDs that have portfolios asynchronously.
        /// </summary>
        /// <returns>A list of user GUIDs.</returns>
        Task<List<Guid>> GetAllUserIdsAsync(); // Assuming user ID is a Guid








        //public Task StoreMarketValueAsync(Guid userId, DateOnly date, decimal marketValue);
        //public Task<decimal?> GetMarketValueOnDateAsync(Guid userId, DateOnly date);

        //public DateTime? GetEarliestMarketValueDate(Guid userId);

        //Task AddPortfolioAsync(Portfolio portfolio);
        //public Task AddPositionToPortfolioAsync( Position position, Guid portfolioId);
        //public Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync();
        //public Task RemovePositionFromPortfolioAsync(Guid portfolioId, string symbol);
        //public Task RemovePortfolioAsync(Guid userId);
        ////Task<IEnumerable<Portfolio>> GetUsersWatchLists(int userId);

        //Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync();
        //Task UpdatePortfolioAsync(Portfolio portfolio);
        //Task RemovePortfolioAsync(int portfolioId);
        //Task AddPositionToPortfolioAsync(int portfolioId, Position position);
        //Task RemovePositionFromPortfolioAsync(int portfolioId, string symbol);



    }
}

