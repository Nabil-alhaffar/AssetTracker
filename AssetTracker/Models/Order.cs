using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
    public sealed record Order
    {
        [BsonId]  // MongoDB will map _id to this property
        public ObjectId MongoId { get; set; } // MongoDB uses ObjectId by default

        [BsonRepresentation(BsonType.String)]
        public Guid OrderId { get; set; } = Guid.NewGuid();

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        [Required]
        public string Symbol { get; set; }
        [Required]
        public decimal Quantity { get; set; }

        public decimal Price { get; set; }
        public OrderType Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderType
    {
        [EnumMember(Value = "BUY")]
        Buy,


        [EnumMember(Value = "SELL")]
        Sell,

        [EnumMember(Value = "SHORT")]
        Short,
        [EnumMember(Value = "CLOSE_SHORT")]

        CloseShort
    }


}

