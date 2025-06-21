using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Models.Alert;
using AssetTracker.Repositories.Interfaces;
using MongoDB.Driver;
using MongoDB.Bson;

namespace AssetTracker.Repositories.MongoDBRepositories
{
    /// <summary>
    /// MongoDB implementation of the <see cref="IAlertRepository"/> interface.
    /// Provides methods to manage alert records in the MongoDB database.
    /// </summary>
    public class MongoAlertRepository : IAlertRepository
    {
        private readonly IMongoCollection<StockAlert> _alertCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoAlertRepository"/> class.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        public MongoAlertRepository(IMongoDatabase database)
        {
            _alertCollection = database.GetCollection<StockAlert>("Alerts");
        }

        /// <summary>
        /// Adds a new alert to the collection.
        /// </summary>
        /// <param name="alert">The alert to add.</param>
        public async Task AddAlertAsync(StockAlert alert)
        {
            alert.CreatedAt = DateTime.UtcNow;
            alert.UpdatedAt = DateTime.UtcNow;
            await _alertCollection.InsertOneAsync(alert);
        }

        /// <summary>
        /// Retrieves an alert by its unique identifier.
        /// </summary>
        /// <param name="alertId">The alert's unique identifier.</param>
        /// <returns>The alert if found; otherwise, null.</returns>
        public async Task<StockAlert?> GetAlertByIdAsync(Guid alertId)
        {
            return await _alertCollection.Find(a => a.AlertId == alertId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all alerts for a specific user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A collection of alerts belonging to the user.</returns>
        public async Task<List<StockAlert>> GetAlertsByUserIdAsync(Guid userId)
        {
            return await _alertCollection.Find(a => a.UserId == userId)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all active alerts for a specific user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A collection of active alerts belonging to the user.</returns>
        public async Task<List<StockAlert>> GetActiveAlertsByUserIdAsync(Guid userId)
        {
            return await _alertCollection.Find(a => a.UserId == userId && a.IsActive)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all alerts for a specific symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A collection of alerts for the symbol.</returns>
        public async Task<List<StockAlert>> GetAlertsBySymbolAsync(string symbol)
        {
            return await _alertCollection.Find(a => a.Symbol == symbol && a.IsActive)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all active alerts.
        /// </summary>
        /// <returns>A collection of all active alerts.</returns>
        public async Task<List<StockAlert>> GetAllActiveAlertsAsync()
        {
            return await _alertCollection.Find(a => a.IsActive)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Updates an existing alert.
        /// </summary>
        /// <param name="alert">The alert to update.</param>
        public async Task UpdateAlertAsync(StockAlert alert)
        {
            alert.UpdatedAt = DateTime.UtcNow;
            await _alertCollection.ReplaceOneAsync(
                a => a.AlertId == alert.AlertId,
                alert,
                new ReplaceOptions { IsUpsert = false }
            );
        }

        /// <summary>
        /// Deletes an alert by its unique identifier.
        /// </summary>
        /// <param name="alertId">The alert's unique identifier.</param>
        public async Task DeleteAlertAsync(Guid alertId)
        {
            await _alertCollection.DeleteOneAsync(a => a.AlertId == alertId);
        }

        /// <summary>
        /// Deletes all alerts for a specific user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        public async Task DeleteAlertsByUserIdAsync(Guid userId)
        {
            await _alertCollection.DeleteManyAsync(a => a.UserId == userId);
        }

        /// <summary>
        /// Retrieves alerts that need to be checked (active and not recently triggered).
        /// </summary>
        /// <param name="symbol">Optional symbol to filter by.</param>
        /// <returns>A collection of alerts that need to be checked.</returns>
        public async Task<List<StockAlert>> GetAlertsToCheckAsync(string? symbol = null)
        {
            var filter = Builders<StockAlert>.Filter.And(
                Builders<StockAlert>.Filter.Eq(a => a.IsActive, true),
                Builders<StockAlert>.Filter.Eq(a => a.IsTriggered, false)
            );

            if (!string.IsNullOrEmpty(symbol))
            {
                filter = Builders<StockAlert>.Filter.And(filter, 
                    Builders<StockAlert>.Filter.Eq(a => a.Symbol, symbol));
            }

            return await _alertCollection.Find(filter)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Marks an alert as triggered.
        /// </summary>
        /// <param name="alertId">The alert's unique identifier.</param>
        public async Task MarkAlertAsTriggeredAsync(Guid alertId)
        {
            var update = Builders<StockAlert>.Update
                .Set(a => a.IsTriggered, true)
                .Set(a => a.LastTriggeredAt, DateTime.UtcNow)
                .Set(a => a.UpdatedAt, DateTime.UtcNow);

            await _alertCollection.UpdateOneAsync(a => a.AlertId == alertId, update);
        }

        /// <summary>
        /// Resets a triggered alert so it can be triggered again.
        /// </summary>
        /// <param name="alertId">The alert's unique identifier.</param>
        public async Task ResetAlertAsync(Guid alertId)
        {
            var update = Builders<StockAlert>.Update
                .Set(a => a.IsTriggered, false)
                .Set(a => a.LastTriggeredAt, (DateTime?)null)
                .Set(a => a.UpdatedAt, DateTime.UtcNow);

            await _alertCollection.UpdateOneAsync(a => a.AlertId == alertId, update);
        }

        /// <summary>
        /// Retrieves triggered alerts for a specific user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="since">Optional date to filter alerts triggered since this time.</param>
        /// <returns>A collection of triggered alerts.</returns>
        public async Task<List<StockAlert>> GetTriggeredAlertsByUserIdAsync(Guid userId, DateTime? since = null)
        {
            var filter = Builders<StockAlert>.Filter.And(
                Builders<StockAlert>.Filter.Eq(a => a.UserId, userId),
                Builders<StockAlert>.Filter.Eq(a => a.IsTriggered, true)
            );

            if (since.HasValue)
            {
                filter = Builders<StockAlert>.Filter.And(filter,
                    Builders<StockAlert>.Filter.Gte(a => a.LastTriggeredAt, since.Value));
            }

            return await _alertCollection.Find(filter)
                .SortByDescending(a => a.LastTriggeredAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves alert statistics for a specific user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>Alert statistics including total, active, and triggered counts.</returns>
        public async Task<AlertStatistics> GetAlertStatisticsByUserIdAsync(Guid userId)
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);

            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("UserId", userId.ToString())),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", (string)null },
                    { "TotalAlerts", new BsonDocument("$sum", 1) },
                    { "ActiveAlerts", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray { "$IsActive", 1, 0 })) },
                    { "TriggeredAlerts", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray { "$IsTriggered", 1, 0 })) },
                    { "TriggeredToday", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray 
                    { 
                        new BsonDocument("$and", new BsonArray 
                        { 
                            "$IsTriggered", 
                            new BsonDocument("$gte", new BsonArray { "$LastTriggeredAt", today })
                        }), 
                        1, 
                        0 
                    })) },
                    { "TriggeredThisWeek", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray 
                    { 
                        new BsonDocument("$and", new BsonArray 
                        { 
                            "$IsTriggered", 
                            new BsonDocument("$gte", new BsonArray { "$LastTriggeredAt", weekStart })
                        }), 
                        1, 
                        0 
                    })) }
                })
            };

            var result = await _alertCollection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();

            if (result == null)
            {
                return new AlertStatistics();
            }

            return new AlertStatistics
            {
                TotalAlerts = result.GetValue("TotalAlerts", 0).AsInt32,
                ActiveAlerts = result.GetValue("ActiveAlerts", 0).AsInt32,
                TriggeredAlerts = result.GetValue("TriggeredAlerts", 0).AsInt32,
                TriggeredToday = result.GetValue("TriggeredToday", 0).AsInt32,
                TriggeredThisWeek = result.GetValue("TriggeredThisWeek", 0).AsInt32
            };
        }
    }
} 