using System;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Repositories.Interfaces
{
    /// <summary>
    /// Interface for managing historical portfolio value data.
    /// </summary>
    public interface IHistoricalPortfolioValueRepository
    {
        /// <summary>
        /// Stores the total portfolio market value for a user on a specific date.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="date">The date for the portfolio value.</param>
        /// <param name="marketValue">The total market value to store.</param>
        Task StoreTotalValueAsync(Guid userId, DateOnly date, decimal marketValue);

        /// <summary>
        /// Retrieves the total portfolio value for a user on a specific date.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="date">The date for which to retrieve the portfolio value.</param>
        /// <returns>The total portfolio value if found; otherwise, null.</returns>
        Task<decimal?> GetTotalValueOnDateAsync(Guid userId, DateOnly date);
    }
}