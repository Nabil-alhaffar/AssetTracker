using System;
using AssetTracker.Repositories.Interfaces;
using AssetTracker.Models;
using AssetTracker.Services.Interfaces;
using Hangfire.Logging;

namespace AssetTracker.Services
{
	public class CashFlowLogService: ICashFlowLogService
	{
		private readonly ICashFlowLogRepository _cashFlowLogRepository;
		public CashFlowLogService(ICashFlowLogRepository cashFlowLogRepository)
		{
			_cashFlowLogRepository = cashFlowLogRepository;
		}

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
        public async Task AddLogAsync(CashFlowLog log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log), "Log cannot be null.");
            }
            await _cashFlowLogRepository.InsertLogAsync(log);

        }

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

