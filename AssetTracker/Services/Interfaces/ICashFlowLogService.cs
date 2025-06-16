using System;
using System.Collections;
using AssetTracker.Models;

namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Interface for managing cash flow log operations such as deposits and withdrawals.
    /// </summary>
    public interface ICashFlowLogService
    {
        /// <summary>
        /// Adds a new cash flow log entry.
        /// </summary>
        /// <param name="log">The cash flow log to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddLogAsync(CashFlowLog log);

        /// <summary>
        /// Retrieves all cash flow logs associated with a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A collection of cash flow logs for the specified user.</returns>
        Task<IEnumerable<CashFlowLog>> GetLogsByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves a single cash flow log by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the cash flow log.</param>
        /// <returns>The corresponding cash flow log entry.</returns>
        Task<CashFlowLog> GetLogByIdAsync(Guid id);

        /// <summary>
        /// Updates an existing cash flow log.
        /// </summary>
        /// <param name="logId">The ID of the log to update.</param>
        /// <param name="log">The updated cash flow log data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateLogAsync(Guid logId, CashFlowLog log);

        /// <summary>
        /// Deletes a cash flow log entry by its ID.
        /// </summary>
        /// <param name="logId">The ID of the log to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteLogAsync(Guid logId);

        /// <summary>
        /// Retrieves all cash flow log entries in the system.
        /// </summary>
        /// <returns>A collection of all cash flow logs.</returns>
        Task<IEnumerable<CashFlowLog>> GetAllLogsAsync();
    }
}