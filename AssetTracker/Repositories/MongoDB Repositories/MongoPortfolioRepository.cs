using AssetTracker.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Repositories.Interfaces;
using MongoDB.Driver.Linq;

namespace AssetTracker.Repositories.MongoDBRepositories
{
    public class MongoPortfolioRepository : IPortfolioRepository
    {
        private readonly IMongoCollection<Portfolio> _portfolioCollection;

        public MongoPortfolioRepository(IMongoDatabase database)
        {
            _portfolioCollection = database.GetCollection<Portfolio>("Portfolios");
        }

        public async Task<Portfolio> GetUserPortfolioAsync(Guid userId)
        {
            var portfolio = await _portfolioCollection.Find(p => p.UserId == userId).FirstOrDefaultAsync();
            if (portfolio == null)
            {
                throw new InvalidOperationException("Portfolio not found.");
            }

            return portfolio;
        }

        public async Task UpdatePortfolioAsync(Portfolio portfolio)
        {
            var result = await _portfolioCollection.ReplaceOneAsync(
                p => p.UserId == portfolio.UserId,
                portfolio,
                new ReplaceOptions { IsUpsert = true } // If the portfolio doesn't exist, it will be inserted
            );

            if (result.MatchedCount == 0 && result.ModifiedCount == 0)
            {
                throw new InvalidOperationException("Portfolio update failed.");
            }
        }

        public async Task<Dictionary<string, Position>> GetPositionsByUserId(Guid userId)
        {
            var portfolio = await GetUserPortfolioAsync(userId); // Reuse the GetUserPortfolioAsync method
            return portfolio.Positions;
        }

        public async Task AddPortfolioAsync(Portfolio portfolio)
        {
            if (portfolio == null) throw new ArgumentNullException(nameof(portfolio));

            await _portfolioCollection.InsertOneAsync(portfolio);
        }

        public async Task<Position> GetUserPositionBySymbol(Guid userId, string symbol)
        {
            var portfolio = await GetUserPortfolioAsync(userId);
            return portfolio.Positions.TryGetValue(symbol, out var position) ? position : null;
        }
        public async Task<List<Guid>> GetAllUserIdsAsync()
        {
            try
            {
                // Assuming Portfolio documents contain a field UserId of type Guid
                var userIds = await _portfolioCollection
                    .AsQueryable()
                    .Select(p => p.UserId) // Select only the UserId field
                    .Distinct()
                    .ToListAsync();

                return userIds;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all user IDs: {ex.Message}");
            }
        }

    }

    // Define the HistoricalMarketValue model to store market value history

}
