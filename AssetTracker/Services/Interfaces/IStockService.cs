using System;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Provides stock trading operations.
    /// </summary>
    public interface IStockService
    {
        /// <summary>
        /// Executes a trade request for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user executing the trade.</param>
        /// <param name="tradeRequest">The trade request details.</param>
        /// <returns>A task that returns a <see cref="TradeResponse"/> indicating the result of the trade execution.</returns>
        Task<TradeResponse> ExecuteTradeAsync(Guid userId, TradeRequest tradeRequest);
    }
}