using System;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Repositories;
using AssetTracker.Services.Interfaces;
using AssetTracker.Repositories.Interfaces;

namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController:ControllerBase
	{
		private readonly IOrderRepository _orderRepository;
		public OrderController(IOrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}
        [HttpGet("Orders/{userId}")]
        public async Task<IActionResult> GetAllUserOrders(Guid userId)
        {
            var orders = await _orderRepository.GetAllUserOrdersAsync(userId);
            if (orders == null)
                return NotFound(new { message = "No data found." });

            return Ok(new { orders });


        }

        [HttpGet("Orders/{userId}/{symbol}")]
        public async Task<IActionResult> GetUserOrdersByPosition(Guid userId, string symbol)
        {
            var orders = await _orderRepository.GetPositionOrdersAsync(userId,symbol);
            if (orders == null)
                return NotFound(new { message = "No data found." });

            return Ok(new { orders });


        }
    }

	
}

