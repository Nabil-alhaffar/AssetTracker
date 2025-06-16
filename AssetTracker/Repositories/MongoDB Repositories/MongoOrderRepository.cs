using AssetTracker.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Repositories.Interfaces;

namespace AssetTracker.Repositories.MongoDBRepositories
{
    /// <summary>
    /// MongoDB implementation of <see cref="IOrderRepository"/> interface.
    /// Provides CRUD operations for user orders stored in MongoDB.
    /// </summary>
    public class MongoOrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orderCollection;

        /// <summary>
        /// Initializes a new instance of <see cref="MongoOrderRepository"/> with the specified MongoDB database.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        public MongoOrderRepository(IMongoDatabase database)
        {
            _orderCollection = database.GetCollection<Order>("Orders");  // "Orders" is the MongoDB collection name
        }

        /// <summary>
        /// Adds a new order asynchronously.
        /// </summary>
        /// <param name="order">The order to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddOrderAsync(Order order)
        {
            await _orderCollection.InsertOneAsync(order);
        }

        /// <summary>
        /// Gets all orders for a specified user asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A list of orders for the user.</returns>
        public async Task<IList<Order>> GetAllUserOrdersAsync(Guid userId)
        {
            var orders = await _orderCollection
                .Find(o => o.UserId == userId)
                .ToListAsync();

            return orders;
        }

        /// <summary>
        /// Gets all orders for a user's specific position (by symbol) asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A list of orders matching the symbol for the user.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no orders are found for the symbol.</exception>
        public async Task<IList<Order>> GetPositionOrdersAsync(Guid userId, string symbol)
        {
            var orders = await _orderCollection
                .Find(o => o.UserId == userId && o.Symbol == symbol)
                .ToListAsync();

            if (orders.Count == 0)
                throw new InvalidOperationException($"User orders do not include any with symbol '{symbol}'.");

            return orders;
        }

        /// <summary>
        /// Updates an existing order asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="updatedOrder">The updated order object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the order to update is not found.</exception>
        public async Task UpdateOrderAsync(Guid orderId, Order updatedOrder)
        {
            var result = await _orderCollection.ReplaceOneAsync(
                o => o.OrderId == orderId,
                updatedOrder
            );

            if (result.MatchedCount == 0)
                throw new InvalidOperationException("Order not found.");
        }

        /// <summary>
        /// Deletes an order asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the order to delete is not found.</exception>
        public async Task DeleteOrderAsync(Guid orderId)
        {
            var result = await _orderCollection.DeleteOneAsync(o => o.OrderId == orderId);

            if (result.DeletedCount == 0)
                throw new InvalidOperationException("Order not found.");
        }
    }
}