using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Provides portfolio-related operations such as retrieving portfolio details, positions, and updating funds.
    /// </summary>
    public interface IPortfolioService
    {
        /// <summary>
        /// Updates the available funds for a user by adding an additional amount.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="additionalAmount">The amount to add to the available funds.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAvailableFundsAsync(Guid userId, decimal additionalAmount);

        /// <summary>
        /// Retrieves the available funds for a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task that returns the available funds as a decimal.</returns>
        Task<decimal> GetAvailableFundsAsync(Guid userId);

        /// <summary>
        /// Retrieves a summary of the user's portfolio.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task that returns a <see cref="PortfolioSummary"/> object.</returns>
        Task<PortfolioSummary> GetPortfolioSummaryAsync(Guid userId);

        /// <summary>
        /// Retrieves the performance metrics of the user's portfolio over a given number of days.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="days">The number of days for performance calculation.</param>
        /// <returns>A task that returns a <see cref="PortfolioPerformance"/> object.</returns>
        Task<PortfolioPerformance> GetPortfolioPerformanceAsync(Guid userId, int days);

        /// <summary>
        /// Retrieves the dictionary of positions keyed by stock symbol for the specified user's portfolio.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task that returns a dictionary of positions keyed by stock symbol.</returns>
        Task<Dictionary<string, Position>> GetPortfolioPositionsAsync(Guid userId);

        /// <summary>
        /// Retrieves a specific position by stock symbol for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="symbol">The stock symbol to look up.</param>
        /// <returns>A task that returns the position if found; otherwise null.</returns>
        Task<Position> GetUserPositionBySymbol(Guid userId, string symbol);

        /// <summary>
        /// Retrieves the full portfolio object for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task that returns the user's <see cref="Portfolio"/>.</returns>
        Task<Portfolio> GetUserPortfolioAsync(Guid userId);

        /// <summary>
        /// Updates total portfolio values for all users.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateTotalValuesForAllUsersAsync();

        /// <summary>
        /// Updates portfolio information for all users.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdatePortfolioForAllUsersAsync();

        /// <summary>
        /// Updates the portfolio data for a specific user by their user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdatePortfolioByUserId(Guid userId);

        // Task<ICollection<Position>> GetAllPositionsAsync(Guid userId);
        // Task AddPositionToPortfolioAsync(Position position, Guid userId);
        // Task RemovePositionAsync(Guid userId, string stockSymbol);
        // Task<Portfolio> GetPortfolioAsync(Guid userId);
    }
}