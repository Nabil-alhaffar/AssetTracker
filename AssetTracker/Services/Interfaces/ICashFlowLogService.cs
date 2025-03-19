using System;
using System.Collections;
using AssetTracker.Models;

namespace AssetTracker.Services.Interfaces
{

	public interface ICashFlowLogService
	{
        public Task AddLogAsync(CashFlowLog log);
        public Task<IEnumerable<CashFlowLog>> GetLogsByUserIdAsync(Guid userId);
        public Task<CashFlowLog> GetLogByIdAsync(Guid id);
        public Task UpdateLogAsync(Guid logId, CashFlowLog log);
        public Task DeleteLogAsync(Guid logId);
        public Task<IEnumerable<CashFlowLog>> GetAllLogsAsync();



    }
}

