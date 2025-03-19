using System;
using AssetTracker.Models;
using AssetTracker.Repositories.Interfaces;
using System.Linq;
using System.Collections.Generic;
using Hangfire.Logging;

namespace AssetTracker.Repositories.MockRepositories
{
	public class CashFlowLogRepository: ICashFlowLogRepository
	{

        private readonly Dictionary<Guid, CashFlowLog>  _cashFlowLogs;
		public CashFlowLogRepository()
		{
            _cashFlowLogs = new();
		}

        public async Task<bool> DeleteLogAsync(Guid id)
        {
            if (_cashFlowLogs.Remove(id))
            {
                await Task.CompletedTask;  // Successfully removed the user
                return true;
            }
            else
            {
                throw new InvalidOperationException("log not found.");
            }
        }

        public async Task<List<CashFlowLog>> GetAllLogsAsync()
        {
            return await Task.FromResult(_cashFlowLogs.Values.ToList());  // Return all users

        }
        public async Task<List<CashFlowLog>> GetAllLogsByUserId(Guid userId)
        {
            return await Task.FromResult(_cashFlowLogs.Values.Where(l => l.UserId == userId).ToList());  // Return all users

        }

        public async Task<CashFlowLog> GetLogByIdAsync(Guid id)
        {
            if (!_cashFlowLogs.ContainsKey(id))
            {
                throw new InvalidOperationException("User already exists.");
            }
            if (_cashFlowLogs[id] != null)
            {
                return await Task.FromResult(_cashFlowLogs[id]);
            }
            else
            {
                throw new InvalidOperationException("log not found.");

            }
        }

        public async Task InsertLogAsync(CashFlowLog log)
        {

            if (_cashFlowLogs.ContainsKey(log.TransactionId))
            {
                throw new InvalidOperationException("User already exists.");
            }

            _cashFlowLogs[log.TransactionId] = log;  // Add user to dictionary using UserId as the key
            await Task.CompletedTask;  // Simulating async task (no database)        }
        }
            public async Task<bool> UpdateLogAsync(Guid id, CashFlowLog log)
        {
            if (_cashFlowLogs.ContainsKey(id))
            {
                _cashFlowLogs[id] = log;  // Update user in the dictionary
                await Task.CompletedTask;  // Simulate async task
                return true;
            }
            else
            {
                throw new InvalidOperationException("Log not found.");
            }
        }
    }
}

