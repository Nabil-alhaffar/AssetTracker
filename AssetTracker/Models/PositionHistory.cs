using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a historical transaction or event related to a position in a user's portfolio.
    /// </summary>
    public sealed record PositionHistory
    {
        /// <summary>
        /// MongoDB unique identifier.
        /// </summary>
        [BsonId]
        public ObjectId MongoId { get; set; }

        /// <summary>
        /// Unique identifier for the position history record.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid PositionHistoryId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The user to whom this position history belongs.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        /// <summary>
        /// The position associated with this history entry.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid PositionId { get; set; }

        /// <summary>
        /// The stock or asset symbol involved in the transaction.
        /// </summary>
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// Date and time of the transaction.
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Type of action performed (e.g., Buy, Sell, Dividend).
        /// </summary>
        public string ActionType { get; set; } = null!;

        /// <summary>
        /// Number of units involved in the transaction.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Price per unit for the transaction.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Total monetary amount of the transaction (Quantity * Price).
        /// </summary>
        public decimal TotalAmount => Quantity * Price;
    }
}