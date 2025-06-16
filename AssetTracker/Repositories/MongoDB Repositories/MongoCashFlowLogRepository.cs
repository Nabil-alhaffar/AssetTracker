using System;
using AssetTracker.Models;
using MongoDB.Driver;
using AssetTracker.Repositories.Interfaces;
namespace AssetTracker.Repositories.MongoDBRepositories
{
    /// <summary>
    /// MongoDB implementation of the <see cref="ICashFlowLogRepository"/> interface.
    /// Provides methods to manage CashFlowLog records in the MongoDB database.
    /// </summary>
    public class MongoCashFlowLogRepository : ICashFlowLogRepository
    {
        private readonly IMongoCollection<CashFlowLog> _cashFlowLogCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoCashFlowLogRepository"/> class.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        public MongoCashFlowLogRepository(IMongoDatabase database)
        {
            _cashFlowLogCollection = database.GetCollection<CashFlowLog>("CashFlowLog");
        }

        /// <summary>
        /// Retrieves all cash flow logs asynchronously.
        /// </summary>
        /// <returns>A list of all cash flow logs.</returns>
        public async Task<List<CashFlowLog>> GetAllLogsAsync()
        {
            return await _cashFlowLogCollection.Find(_ => true).ToListAsync();
        }

        /// <summary>
        /// Retrieves all cash flow logs for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A list of cash flow logs belonging to the specified user.</returns>
        public async Task<List<CashFlowLog>> GetAllLogsByUserId(Guid userId)
        {
            var filter = Builders<CashFlowLog>.Filter.Eq(log => log.UserId, userId);
            return await _cashFlowLogCollection.Find(filter).ToListAsync();
        }


        /// <summary>
        /// Retrieves a cash flow log by its transaction ID asynchronously.
        /// </summary>
        /// <param name="id">The unique transaction ID of the log.</param>
        /// <returns>The matching <see cref="CashFlowLog"/> or null if not found.</returns>
        public async Task<CashFlowLog> GetLogByIdAsync(Guid id)
        {
            return await _cashFlowLogCollection.Find(c => c.TransactionId == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Inserts a new cash flow log into the database asynchronously.
        /// </summary>
        /// <param name="log">The <see cref="CashFlowLog"/> to insert.</param>
        public async Task InsertLogAsync(CashFlowLog log)
        {
            await _cashFlowLogCollection.InsertOneAsync(log);
        }


        /// <summary>
        /// Updates an existing cash flow log identified by transaction ID asynchronously.
        /// </summary>
        /// <param name="id">The transaction ID of the log to update.</param>
        /// <param name="log">The updated <see cref="CashFlowLog"/> object.</param>
        /// <returns>True if an existing log was updated; otherwise, false.</returns>
        public async Task<bool> UpdateLogAsync(Guid id, CashFlowLog log)
        {
            var result = await _cashFlowLogCollection.ReplaceOneAsync(c => c.TransactionId == id, log);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Deletes a cash flow log identified by transaction ID asynchronously.
        /// </summary>
        /// <param name="id">The transaction ID of the log to delete.</param>
        /// <returns>True if a log was deleted; otherwise, false.</returns>
        public async Task<bool> DeleteLogAsync(Guid id)
        {
            var result = await _cashFlowLogCollection.DeleteOneAsync(c => c.TransactionId == id);
            return result.DeletedCount > 0;
        }
    }

}

