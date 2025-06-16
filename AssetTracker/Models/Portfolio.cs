using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a user's investment portfolio, including positions and available funds.
    /// </summary>
    public sealed record Portfolio
    {
        /// <summary>
        /// Gets or sets the MongoDB ObjectId used as the primary key.
        /// </summary>
        [BsonId]
        public ObjectId MongoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who owns the portfolio.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the collection of positions keyed by stock symbol.
        /// </summary>
        public Dictionary<string, Position> Positions { get; set; } = new();

        /// <summary>
        /// Gets or sets the amount of funds available for trading.
        /// </summary>
        public decimal AvailableFunds { get; set; } = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Portfolio"/> class.
        /// </summary>
        public Portfolio()
        {
            Positions = new Dictionary<string, Position>();
        }
    }
}