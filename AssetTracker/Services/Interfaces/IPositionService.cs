using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Provides position management operations such as splitting positions, updating profit/loss, and retrieving position data.
    /// </summary>
    public interface IPositionService
    {
        /// <summary>
        /// Splits a user's position based on the specified split factor.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="symbol">The stock symbol of the position to split.</param>
        /// <param name="splitFactor">The factor by which the position is split.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SplitPositionAsync(Guid userId, string symbol, int splitFactor);

        /// <summary>
        /// Checks whether the specified position has triggered its stop loss price.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="symbol">The stock symbol of the position to check.</param>
        /// <param name="stopLossPrice">The stop loss price to check against.</param>
        /// <returns>A task that returns true if the stop loss was triggered; otherwise false.</returns>
        Task<bool> CheckPositionForStopLossAsync(Guid userId, string symbol, decimal stopLossPrice);

        /// <summary>
        /// Updates the profit and loss information for a given position.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="symbol">The stock symbol of the position to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdatePositionProfitLossAsync(Guid userId, string symbol);

        /// <summary>
        /// Adds a position history record.
        /// </summary>
        /// <param name="history">The position history record to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddPositionHistoryAsync(PositionHistory history);

        /// <summary>
        /// Retrieves a summary of a position for a user and symbol.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="symbol">The stock symbol of the position.</param>
        /// <returns>A task that returns the position summary.</returns>
        Task<PositionSummary> GetPositionSummaryAsync(Guid userId, string symbol);

        /// <summary>
        /// Retrieves the position history for a user and stock symbol.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A task that returns a list of position history records.</returns>
        Task<List<PositionHistory>> GetPositionHistoryAsync(Guid userId, string symbol);

        /// <summary>
        /// Retrieves the current position for a user and stock symbol.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A task that returns the position if found; otherwise null.</returns>
        Task<Position> GetPositionAsync(Guid userId, string symbol);

        /// <summary>
        /// Updates a position based on the given order.
        /// </summary>
        /// <param name="order">The order information used to update the position.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdatePositionAsync(Order order);

        // /// <summary>
        // /// Adds or updates a position in the portfolio.
        // /// </summary>
        // /// <param name="userId">The unique identifier of the user.</param>
        // /// <param name="symbol">The stock symbol.</param>
        // /// <param name="quantity">The quantity of shares.</param>
        // /// <param name="price">The price per share.</param>
        // /// <returns>A task representing the asynchronous operation.</returns>
        // Task AddOrUpdatePositionAsync(Guid userId, string symbol, decimal quantity, decimal price);

        // /// <summary>
        // /// Reduces or removes a position from the portfolio.
        // /// </summary>
        // /// <param name="userId">The unique identifier of the user.</param>
        // /// <param name="symbol">The stock symbol.</param>
        // /// <param name="quantity">The quantity to reduce.</param>
        // /// <param name="price">The price per share.</param>
        // /// <returns>A task representing the asynchronous operation.</returns>
        // Task ReduceOrRemovePositionAsync(Guid userId, string symbol, decimal quantity, decimal price);

        // /// <summary>
        // /// Adds or updates a short position in the portfolio.
        // /// </summary>
        // /// <param name="userId">The unique identifier of the user.</param>
        // /// <param name="symbol">The stock symbol.</param>
        // /// <param name="quantity">The quantity of shares.</param>
        // /// <param name="price">The price per share.</param>
        // /// <returns>A task representing the asynchronous operation.</returns>
        // Task AddOrUpdateShortPositionAsync(Guid userId, string symbol, decimal quantity, decimal price);
    }
}