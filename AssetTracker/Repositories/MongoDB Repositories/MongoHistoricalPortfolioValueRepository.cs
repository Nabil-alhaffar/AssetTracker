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

        public async Task StoreMarketValueAsync(Guid userId, DateOnly date, decimal marketValue)
        {
            var historicalMarketValue = new HistoricalPortfolioValue
            {
                UserId = userId,
                Date = date,
                MarketValue = marketValue
            };

            await _historicalPortfolioValueCollection.InsertOneAsync(historicalMarketValue);
        }

        public async Task<decimal?> GetMarketValueOnDateAsync(Guid userId, DateOnly date)
        {
            var marketValue = await _historicalPortfolioValueCollection
                .Find(h => h.UserId == userId && h.Date == date)
                .FirstOrDefaultAsync();

            return marketValue?.MarketValue;
        }
    }
}

