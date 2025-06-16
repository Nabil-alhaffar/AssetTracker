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
    /// <summary>
    /// MongoDB implementation of the <see cref="IPortfolioRepository"/> interface.
    /// Provides methods to manage portfolio records in the MongoDB database including positions and available cash.
    /// </summary>
    public class MongoPortfolioRepository : IPortfolioRepository
    {
        private readonly IMongoCollection<Portfolio> _portfolioCollection;

        /// <summary>
        /// Initializes a new instance of <see cref="MongoPortfolioRepository"/> with the specified MongoDB database.
        /// </summary>
        /// <param name="database">MongoDB database instance.</param>
        public MongoPortfolioRepository(IMongoDatabase database)
        {
            _portfolioCollection = database.GetCollection<Portfolio>("Portfolios");
        }


        /// <summary>
        /// Retrieves the portfolio of the specified user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>The user's portfolio.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the portfolio is not found.</exception>
        public async Task<Portfolio> GetUserPortfolioAsync(Guid userId)
        {
            var portfolio = await _portfolioCollection.Find(p => p.UserId == userId).FirstOrDefaultAsync();
            if (portfolio == null)
            {
                throw new InvalidOperationException("Portfolio not found.");
            }

            return portfolio;
        }


        /// <summary>
        /// Updates the portfolio of the user. Inserts if it doesn't exist.
        /// </summary>
        /// <param name="portfolio">The portfolio object to update or insert.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the update fails.</exception>
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

        

        /// <summary>
        /// Gets all positions for the user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A dictionary mapping stock symbols to positions.</returns>
        public async Task<Dictionary<string, Position>> GetPositionsByUserId(Guid userId)
        {
            var portfolio = await GetUserPortfolioAsync(userId); // Reuse the GetUserPortfolioAsync method
            return portfolio.Positions;
        }

        /// <summary>
        /// Adds a new portfolio document.
        /// </summary>
        /// <param name="portfolio">The portfolio to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddPortfolioAsync(Portfolio portfolio)
        {
            if (portfolio == null) throw new ArgumentNullException(nameof(portfolio));

            await _portfolioCollection.InsertOneAsync(portfolio);
        }

        /// <summary>
        /// Retrieves a specific position by symbol from the user's portfolio.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The position if found; otherwise, null.</returns>
        public async Task<Position> GetUserPositionBySymbol(Guid userId, string symbol)
        {
            var portfolio = await GetUserPortfolioAsync(userId);
            return portfolio.Positions.TryGetValue(symbol, out var position) ? position : null;
        }

        

        /// <summary>
        /// Retrieves a list of all user IDs with portfolios.
        /// </summary>
        /// <returns>A list of user IDs.</returns>
        /// <exception cref="Exception">Throws if the query fails.</exception>
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


}
