
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using AssetTracker.Models.Enums;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using AssetTracker.Helpers;
namespace AssetTracker.Models
{
    public sealed record CashFlowLog
    {
        [BsonId]  // MongoDB will map _id to this property
        public ObjectId MongoId { get; set; } // MongoDB uses ObjectId by default

        [BsonRepresentation(BsonType.String)]
        public Guid TransactionId { get; set; } = Guid.NewGuid();

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        //public DateTime LocalTimeStamp  => TimezoneHelper.ConvertUtcToLocal(Timestamp);

    }

}

