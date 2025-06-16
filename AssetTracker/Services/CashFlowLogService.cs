using System;
using AssetTracker.Repositories.Interfaces;
using AssetTracker.Models;
using AssetTracker.Services.Interfaces;
using Hangfire.Logging;

namespace AssetTracker.Services
{

    
    /// <summary>
    /// Service for managing cash flow logs.
    /// </summary>
	public class CashFlowLogService: ICashFlowLogService
	{
		private readonly ICashFlowLogRepository _cashFlowLogRepository;
		public CashFlowLogService(ICashFlowLogRepository cashFlowLogRepository)
		{
			_cashFlowLogRepository = cashFlowLogRepository;
		}

        

        /// <summary>
        /// Retrieves all cash flow logs.
        /// </summary>
        /// <returns>A collection of all <see cref="CashFlowLog"/> entries.</returns>
        public async Task<IEnumerable<CashFlowLog>> GetAllLogsAsync()
        {
            try
            {
                var log = await _cashFlowLogRepository.GetAllLogsAsync();
                return log;
            }

            catch (Exception ex)
            {
                throw new Exception( "Error fetching Logs", ex);

            }
        }



        /// <summary>
        /// Retrieves a specific cash flow log by transaction ID.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction.</param>
        /// <returns>The <see cref="CashFlowLog"/> entry matching the given ID.</returns>
        public async Task<CashFlowLog> GetLogByIdAsync(Guid transactionId)
        {
            try
            {
                var log = await _cashFlowLogRepository.GetLogByIdAsync(transactionId);
                return log;
            }

            catch(Exception ex)
            {
                throw new ArgumentNullException(nameof(transactionId), ex);

            }
        }


        /// <summary>
        /// Retrieves all cash flow logs for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A collection of <see cref="CashFlowLog"/> entries associated with the user.</returns>
        public async Task<IEnumerable<CashFlowLog>> GetLogsByUserIdAsync(Guid userId)
        {
            try
            {
                var log = await _cashFlowLogRepository.GetAllLogsByUserId(userId);
                return log;
            }

            catch (Exception ex)
            {
                throw new ArgumentNullException(nameof(userId), ex);

            }
        }

        

        /// <summary>
        /// Adds a new cash flow log.
        /// </summary>
        /// <param name="log">The <see cref="CashFlowLog"/> object to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddLogAsync(CashFlowLog log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log), "Log cannot be null.");
            }
            await _cashFlowLogRepository.InsertLogAsync(log);

        }


        /// <summary>
        /// Deletes a specific cash flow log by transaction ID.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteLogAsync(Guid transactionId)
        {
            try
            {
                await _cashFlowLogRepository.DeleteLogAsync(transactionId);
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(nameof(transactionId), ex);

            }
        }


        /// <summary>
        /// Updates a specific cash flow log entry.
        /// </summary>
        /// <param name="logId">The ID of the log to update.</param>
        /// <param name="log">The updated <see cref="CashFlowLog"/> object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateLogAsync(Guid logId,  CashFlowLog log)
        {
            try
            {
                await _cashFlowLogRepository.UpdateLogAsync(logId, log);
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(nameof(log), ex);

            }
        }

    }
}

