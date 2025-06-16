using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a cash flow log entry for deposits, withdrawals, fees, or other transactions.
    /// </summary>
    public sealed record CashFlowLog
    {
        /// <summary>
        /// Gets or sets the MongoDB ObjectId for this document.
        /// </summary>
        [BsonId]  // MongoDB will map _id to this property
        public ObjectId MongoId { get; set; } // MongoDB uses ObjectId by default

        /// <summary>
        /// Gets or sets the unique identifier for the transaction.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid TransactionId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the unique identifier of the user performing the transaction.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the amount involved in the transaction.
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets an optional description for the transaction.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the type of transaction (Deposit, Withdrawal, Fee, etc.).
        /// </summary>
        [Required]
        public TransactionType Type { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the transaction was recorded.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Gets local timestamp converted from UTC based on user's timezone
        // public DateTime LocalTimeStamp => TimezoneHelper.ConvertUtcToLocal(Timestamp);
    }
}