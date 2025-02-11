using System;
using System.Collections.Generic;
using AssetTracker.Models;
namespace AssetTracker.Repositories
{
	public class OrderRepository:IOrderRepository
	{
        private readonly Dictionary<Guid, IList< Order>> _userOrders;

        public OrderRepository()
		{
			_userOrders = new();
		}

        public async Task AddOrderAsync(Order order)
        {
            if (!_userOrders.ContainsKey(order.UserId))
                _userOrders[order.UserId] = new List<Order>();
            
            
            _userOrders[order.UserId].Add(order);
            await Task.CompletedTask;
        }

        public async Task<IList<Order>> GetAllUserOrdersAsync(Guid userId)
        {
            await Task.Delay(10); // Simulated delay for async operation

            if (!_userOrders.ContainsKey(userId))
            {
                return new List<Order>();

            }
            return _userOrders[userId].ToList();

            
        }

        public async Task<IList<Order>> GetPositionOrdersAsync(Guid userId, string symbol)
        {
            await Task.Delay(10); // Simulated delay for async operation

            if (!_userOrders.ContainsKey(userId))
            {
                throw new InvalidOperationException("User Orders not found.");

            }
            var positionOrders =_userOrders[userId].Where(p => p.Symbol == symbol).ToList();
            if (positionOrders.Count() ==0)
                throw new InvalidOperationException($"User orders does not include any with {symbol}");

            return positionOrders;
        }
    }
}

