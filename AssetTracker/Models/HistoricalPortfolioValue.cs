using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace AssetTracker.Models
{
    public sealed record HistoricalPortfolioValue
    {
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        public DateOnly Date { get; set; }
        public decimal MarketValue { get; set; }
    }
}

