using System;
using MongoDB.Driver;
using AssetTracker.Models;
using AssetTracker.Repositories.Interfaces;
namespace AssetTracker.Repositories.MongoDBRepositories
{
    /// <summary>
    /// MongoDB implementation of the <see cref="IHistoricalPortfolioValueRepository"/> interface.
    /// Provides methods to store and retrieve historical portfolio values.
    /// </summary>
    public class MongoHistoricalPortfolioValueRepository: IHistoricalPortfolioValueRepository
    {
        private readonly IMongoCollection<HistoricalPortfolioValue> _historicalPortfolioValueCollection;


        /// <summary>
        /// Initializes a new instance of the <see cref="MongoHistoricalPortfolioValueRepository"/> class.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        public MongoHistoricalPortfolioValueRepository(IMongoDatabase database)
        {
            _historicalPortfolioValueCollection = database.GetCollection<HistoricalPortfolioValue>("HistoricalPortfolioValues");
        }


        /// <summary>
        /// Stores the total portfolio market value for a user on a specific date asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="date">The date for which the market value is recorded.</param>
        /// <param name="marketValue">The total market value of the portfolio.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StoreTotalValueAsync(Guid userId, DateOnly date, decimal marketValue)
        {
            var historicalMarketValue = new HistoricalPortfolioValue
            {
                UserId = userId,
                Date = date,
                TotalValue = marketValue
            };

            await _historicalPortfolioValueCollection.InsertOneAsync(historicalMarketValue);
        }

        /// <summary>
        /// Retrieves the total portfolio market value for a user on a specific date asynchronously.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="date">The date for which to retrieve the market value.</param>
        /// <returns>The total market value if found; otherwise, null.</returns>
        public async Task<decimal?> GetTotalValueOnDateAsync(Guid userId, DateOnly date)
        {
            var totalValue = await _historicalPortfolioValueCollection
                .Find(h => h.UserId == userId && h.Date == date)
                .FirstOrDefaultAsync();

            return totalValue?.TotalValue;
        }
    }
}

