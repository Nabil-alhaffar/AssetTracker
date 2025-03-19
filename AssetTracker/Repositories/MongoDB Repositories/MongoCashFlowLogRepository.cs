using System;
using AssetTracker.Models;
using MongoDB.Driver;
using AssetTracker.Repositories.Interfaces;
namespace AssetTracker.Repositories.MongoDBRepositories
{

    public class MongoCashFlowLogRepository : ICashFlowLogRepository
    {
        private readonly IMongoCollection<CashFlowLog> _cashFlowLogCollection;

        public MongoCashFlowLogRepository(IMongoDatabase database)
        {
            _cashFlowLogCollection = database.GetCollection<CashFlowLog>("CashFlowLog");
        }

        public async Task<List<CashFlowLog>> GetAllLogsAsync()
        {
            return await _cashFlowLogCollection.Find(_ => true).ToListAsync();
        }
        public async Task<List<CashFlowLog>> GetAllLogsByUserId(Guid userId)
        {
            var filter = Builders<CashFlowLog>.Filter.Eq(log => log.UserId, userId);
            return await _cashFlowLogCollection.Find(filter).ToListAsync();
        }

        public async Task<CashFlowLog> GetLogByIdAsync(Guid id)
        {
            return await _cashFlowLogCollection.Find(c => c.TransactionId == id).FirstOrDefaultAsync();
        }

        public async Task InsertLogAsync(CashFlowLog log)
        {
            await _cashFlowLogCollection.InsertOneAsync(log);
        }

        public async Task<bool> UpdateLogAsync(Guid id, CashFlowLog log)
        {
            var result = await _cashFlowLogCollection.ReplaceOneAsync(c => c.TransactionId == id, log);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteLogAsync(Guid id)
        {
            var result = await _cashFlowLogCollection.DeleteOneAsync(c => c.TransactionId == id);
            return result.DeletedCount > 0;
        }
    }

}

