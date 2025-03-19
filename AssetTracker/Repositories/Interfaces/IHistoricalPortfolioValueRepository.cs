using System;
using AssetTracker.Models;

namespace AssetTracker.Repositories.Interfaces
{
	public interface IHistoricalPortfolioValueRepository
	{
        public  Task StoreTotalValueAsync(Guid userId, DateOnly date, decimal marketValue);

        public  Task<decimal?> GetTotalValueOnDateAsync(Guid userId, DateOnly date);

    }
}

