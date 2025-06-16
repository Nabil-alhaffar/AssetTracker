using System;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Repositories;
using AssetTracker.Services.Interfaces;
using AssetTracker.Repositories.Interfaces;

namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        // Constructor injection of IOrderRepository
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Gets all orders for a specific user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <returns>List of orders or a 404 if none found</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllUserOrders(Guid userId)
        {
            var orders = await _orderRepository.GetAllUserOrdersAsync(userId);

            if (orders == null)
                return NotFound(new { message = "No data found." });

            return Ok(new { orders });
        }

        /// <summary>
        /// Gets all orders for a specific user and symbol (i.e., for a particular position).
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="symbol">Stock symbol</param>
        /// <returns>List of orders or 404 if none found</returns>
        [HttpGet("{userId}/{symbol}")]
        public async Task<IActionResult> GetUserOrdersByPosition(Guid userId, string symbol)
        {
            var orders = await _orderRepository.GetPositionOrdersAsync(userId, symbol);

            if (orders == null)
                return NotFound(new { message = "No data found." });

            return Ok(new { orders });
        }
    }
}