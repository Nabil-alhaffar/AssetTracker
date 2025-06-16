using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the historical total value of a user's portfolio on a specific date.
    /// </summary>
    public sealed record HistoricalPortfolioValue
    {
        /// <summary>
        /// Gets or sets the MongoDB ObjectId.
        /// </summary>
        [BsonId]  // MongoDB will map _id to this property
        public ObjectId MongoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the date the portfolio value was recorded.
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Gets or sets the total value of the portfolio on the specified date.
        /// </summary>
        public decimal TotalValue { get; set; }
    }
}