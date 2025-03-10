using AssetTracker.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Repositories.Interfaces;


namespace AssetTracker.Repositories.MongoDBRepositories
{
    public class MongoOrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orderCollection;

        // Constructor to initialize the MongoDB collection
        public MongoOrderRepository(IMongoDatabase database)
        {
            _orderCollection = database.GetCollection<Order>("Orders");  // "Orders" is the MongoDB collection name
        }

        // Add a new order
        public async Task AddOrderAsync(Order order)
        {
            // Insert the order into MongoDB
            await _orderCollection.InsertOneAsync(order);
        }

        // Get all orders for a user
        public async Task<IList<Order>> GetAllUserOrdersAsync(Guid userId)
        {
            var orders = await _orderCollection
                .Find(o => o.UserId == userId)  // Filter by UserId
                .ToListAsync();

            return orders;
        }

        // Get all orders for a user's specific position (symbol)
        public async Task<IList<Order>> GetPositionOrdersAsync(Guid userId, string symbol)
        {
            var orders = await _orderCollection
                .Find(o => o.UserId == userId && o.Symbol == symbol)  // Filter by UserId and Symbol
                .ToListAsync();

            if (orders.Count == 0)
                throw new InvalidOperationException($"User orders does not include any with {symbol}");

            return orders;
        }

        // Update an existing order
        public async Task UpdateOrderAsync(Guid orderId, Order updatedOrder)
        {
            var result = await _orderCollection.ReplaceOneAsync(
                o => o.OrderId == orderId,  // Match the order by Id
                updatedOrder  // Replace the old order with the updated order
            );

            if (result.MatchedCount == 0)
                throw new InvalidOperationException("Order not found.");
        }

        // Delete an order
        public async Task DeleteOrderAsync(Guid orderId)
        {
            var result = await _orderCollection.DeleteOneAsync(o => o.OrderId == orderId);  // Delete order by Id

            if (result.DeletedCount == 0)
                throw new InvalidOperationException("Order not found.");
        }
    }
}
