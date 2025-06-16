using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using AssetTracker.Helpers;
using AssetTracker.Models.Enums;

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
        public string Symbol { get; set; } = null!;
        [Required]
        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public OrderSide Side { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;



    }




}

