using System;
using AssetTracker.Models;
using MongoDB.Driver;

namespace AssetTracker.Repositories.Interfaces
{
	public interface ICashFlowLogRepository
	{


        public Task<List<CashFlowLog>> GetAllLogsAsync();

        public Task<List<CashFlowLog>> GetAllLogsByUserId(Guid userId);

        public Task<CashFlowLog> GetLogByIdAsync(Guid id);


        public Task InsertLogAsync(CashFlowLog log);


        public Task<bool> UpdateLogAsync(Guid id, CashFlowLog log);


        public Task<bool> DeleteLogAsync(Guid id);




    }
}

