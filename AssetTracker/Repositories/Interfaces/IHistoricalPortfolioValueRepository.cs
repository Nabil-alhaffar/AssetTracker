using System;
using AssetTracker.Models;

namespace AssetTracker.Repositories.Interfaces
{
	public interface IHistoricalPortfolioValueRepository
	{
        public  Task StoreMarketValueAsync(Guid userId, DateOnly date, decimal marketValue);

        public  Task<decimal?> GetMarketValueOnDateAsync(Guid userId, DateOnly date);

    }
}

