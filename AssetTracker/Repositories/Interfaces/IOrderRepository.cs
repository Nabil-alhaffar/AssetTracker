using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Repositories.Interfaces
{
    /// <summary>
    /// Interface for managing order data persistence.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Adds a new order asynchronously.
        /// </summary>
        /// <param name="order">The order to add.</param>
        Task AddOrderAsync(Order order);

        /// <summary>
        /// Retrieves all orders for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A list of orders associated with the user.</returns>
        Task<IList<Order>> GetAllUserOrdersAsync(Guid userId);

        /// <summary>
        /// Retrieves all orders for a user related to a specific stock symbol asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A list of orders for the given symbol.</returns>
        Task<IList<Order>> GetPositionOrdersAsync(Guid userId, string symbol);
    }
}