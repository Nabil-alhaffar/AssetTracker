using System;
using MongoDB.Driver;
using AssetTracker.Models;
using AssetTracker.Repositories.Interfaces;
namespace AssetTracker.Repositories.MongoDBRepositories
{
    public class MongoHistoricalPortfolioValueRepository: IHistoricalPortfolioValueRepository
    {
        private readonly IMongoCollection<HistoricalPortfolioValue> _historicalPortfolioValueCollection;

        public MongoHistoricalPortfolioValueRepository(IMongoDatabase database)
        {
            _historicalPortfolioValueCollection = database.GetCollection<HistoricalPortfolioValue>("HistoricalPortfolioValues");
        }

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

        public async Task<decimal?> GetTotalValueOnDateAsync(Guid userId, DateOnly date)
        {
            var totalValue = await _historicalPortfolioValueCollection
                .Find(h => h.UserId == userId && h.Date == date)
                .FirstOrDefaultAsync();

            return totalValue?.TotalValue;
        }
    }
}

