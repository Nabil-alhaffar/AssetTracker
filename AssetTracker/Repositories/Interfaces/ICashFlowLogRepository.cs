using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Repositories.Interfaces
{
    /// <summary>
    /// Interface for CRUD operations on CashFlowLog entities.
    /// </summary>
    public interface ICashFlowLogRepository
    {
        /// <summary>
        /// Retrieves all cash flow logs.
        /// </summary>
        /// <returns>List of all CashFlowLog entries.</returns>
        Task<List<CashFlowLog>> GetAllLogsAsync();

        /// <summary>
        /// Retrieves all cash flow logs for a specific user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List of CashFlowLog entries belonging to the user.</returns>
        Task<List<CashFlowLog>> GetAllLogsByUserId(Guid userId);

        /// <summary>
        /// Retrieves a cash flow log by its unique identifier.
        /// </summary>
        /// <param name="id">The log identifier.</param>
        /// <returns>The CashFlowLog with the specified id or null if not found.</returns>
        Task<CashFlowLog> GetLogByIdAsync(Guid id);

        /// <summary>
        /// Inserts a new cash flow log.
        /// </summary>
        /// <param name="log">The cash flow log to insert.</param>
        Task InsertLogAsync(CashFlowLog log);

        /// <summary>
        /// Updates an existing cash flow log by its id.
        /// </summary>
        /// <param name="id">The identifier of the log to update.</param>
        /// <param name="log">The updated log data.</param>
        /// <returns>True if update was successful, false otherwise.</returns>
        Task<bool> UpdateLogAsync(Guid id, CashFlowLog log);

        /// <summary>
        /// Deletes a cash flow log by its id.
        /// </summary>
        /// <param name="id">The identifier of the log to delete.</param>
        /// <returns>True if delete was successful, false otherwise.</returns>
        Task<bool> DeleteLogAsync(Guid id);
    }
}