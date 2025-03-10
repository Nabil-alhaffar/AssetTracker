using System;
using AssetTracker.Models;
namespace AssetTracker.Repositories.Interfaces
{
	public interface IOrderRepository
	{
		Task AddOrderAsync(Order order);
		Task <IList<Order>> GetAllUserOrdersAsync(Guid userId);
		Task<IList<Order>> GetPositionOrdersAsync(Guid userId, string symbol);

		
	}
}

