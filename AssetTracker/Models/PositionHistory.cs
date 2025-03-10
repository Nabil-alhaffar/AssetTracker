using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
    public sealed record PositionHistory
    {
        [BsonId]  // MongoDB will map _id to this property
        public ObjectId MongoId { get; set; } // MongoDB uses ObjectId by default
        public Guid PositionHistoryId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // Associate history with a user
        public Guid PositionId { get; set; }
        public string Symbol { get; set; } // ✅ Added symbol field
        public DateTime TransactionDate { get; set; }
        public string ActionType { get; set; } // Buy, Sell, Dividend, etc.
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount => Quantity * Price;

    }

}

