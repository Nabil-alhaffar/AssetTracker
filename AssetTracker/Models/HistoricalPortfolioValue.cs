using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace AssetTracker.Models
{
    public sealed record HistoricalPortfolioValue
    {
        [BsonId]  // MongoDB will map _id to this property
        public ObjectId MongoId { get; set; } // MongoDB uses ObjectId by default

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        public DateOnly Date { get; set; }
        //public decimal MarketValue { get; set; }
        public decimal TotalValue { get; set; }
    }
}

