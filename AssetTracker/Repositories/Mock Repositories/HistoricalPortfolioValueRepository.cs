using System;
using System.Collections.Generic;
using AssetTracker.Models;
using AssetTracker.Repositories.Interfaces;
namespace AssetTracker.Repositories.MockRepositories
{
	public class HistoricalPortfolioValueRepository:IHistoricalPortfolioValueRepository
	{
        private readonly Dictionary<Guid, SortedDictionary<DateOnly, decimal>> _historicalMarketValues;

        public HistoricalPortfolioValueRepository()
		{
            _historicalMarketValues = new Dictionary<Guid, SortedDictionary<DateOnly, decimal>>();

        }
        public Task StoreMarketValueAsync(Guid userId, DateOnly date, decimal marketValue)
        {
            if (!_historicalMarketValues.ContainsKey(userId))
                _historicalMarketValues[userId] = new SortedDictionary<DateOnly, decimal>();

            _historicalMarketValues[userId][date] = marketValue; // Store only date part
            return Task.CompletedTask;
        }

        public Task<decimal?> GetMarketValueOnDateAsync(Guid userId, DateOnly date)
        {
            if (_historicalMarketValues.TryGetValue(userId, out var marketValues) &&
                marketValues.TryGetValue(date, out var value))
            {
                return Task.FromResult<decimal?>(value);
            }
            return Task.FromResult<decimal?>(null);
        }
    }
}

