
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;
using System.Runtime.Serialization;

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



    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType {
        [EnumMember(Value = "DEPOSIT")]
        Deposit,

        [EnumMember(Value = "WITHDRAWAL")]
        Withdrawal,

        [EnumMember(Value = "TRANSFER")]
        Transfer,

        [EnumMember(Value = "FEE")]
        Fee,

        [EnumMember(Value = "INTEREST")]
        Interest,

        [EnumMember(Value = "ADJUSTMENT")]
        Adjustment,
    }
}

