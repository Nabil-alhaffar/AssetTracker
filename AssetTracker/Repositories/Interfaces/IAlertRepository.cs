using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models.Alert;

namespace AssetTracker.Repositories.Interfaces
{
    /// <summary>
    /// Interface for alert data persistence and retrieval operations.
    /// </summary>
    public interface IAlertRepository
    {
        /// <summary>
        /// Adds a new alert asynchronously.
        /// </summary>
        /// <param name="alert">The alert to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAlertAsync(StockAlert alert);

        /// <summary>
        /// Retrieves an alert by its unique identifier asynchronously.
        /// </summary>
        /// <param name="alertId">The alert's unique identifier.</param>
        /// <returns>The alert if found; otherwise, null.</returns>
        Task<StockAlert?> GetAlertByIdAsync(Guid alertId);

        /// <summary>
        /// Retrieves all alerts for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A collection of alerts belonging to the user.</returns>
        Task<List<StockAlert>> GetAlertsByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves all active alerts for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A collection of active alerts belonging to the user.</returns>
        Task<List<StockAlert>> GetActiveAlertsByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves all alerts for a specific symbol asynchronously.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A collection of alerts for the symbol.</returns>
        Task<List<StockAlert>> GetAlertsBySymbolAsync(string symbol);

        /// <summary>
        /// Retrieves all active alerts asynchronously.
        /// </summary>
        /// <returns>A collection of all active alerts.</returns>
        Task<List<StockAlert>> GetAllActiveAlertsAsync();

        /// <summary>
        /// Updates an existing alert asynchronously.
        /// </summary>
        /// <param name="alert">The alert to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAlertAsync(StockAlert alert);

        /// <summary>
        /// Deletes an alert by its unique identifier asynchronously.
        /// </summary>
        /// <param name="alertId">The alert's unique identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAlertAsync(Guid alertId);

        /// <summary>
        /// Deletes all alerts for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAlertsByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves alerts that need to be checked (active and not recently triggered) asynchronously.
        /// </summary>
        /// <param name="symbol">Optional symbol to filter by.</param>
        /// <returns>A collection of alerts that need to be checked.</returns>
        Task<List<StockAlert>> GetAlertsToCheckAsync(string? symbol = null);

        /// <summary>
        /// Marks an alert as triggered asynchronously.
        /// </summary>
        /// <param name="alertId">The alert's unique identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkAlertAsTriggeredAsync(Guid alertId);

        /// <summary>
        /// Resets a triggered alert so it can be triggered again asynchronously.
        /// </summary>
        /// <param name="alertId">The alert's unique identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ResetAlertAsync(Guid alertId);

        /// <summary>
        /// Retrieves triggered alerts for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="since">Optional date to filter alerts triggered since this time.</param>
        /// <returns>A collection of triggered alerts.</returns>
        Task<List<StockAlert>> GetTriggeredAlertsByUserIdAsync(Guid userId, DateTime? since = null);

        /// <summary>
        /// Retrieves alert statistics for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>Alert statistics including total, active, and triggered counts.</returns>
        Task<AlertStatistics> GetAlertStatisticsByUserIdAsync(Guid userId);
    }


} 