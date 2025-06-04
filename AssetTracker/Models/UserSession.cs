using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
    public class UserSession
    {
        [BsonId]  // MongoDB will map _id to this property
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("EndedAt")]
        public DateTime? EndedAt { get; set; }

        public string IpAddress { get; set; }  // optional

        [JsonPropertyName("userAgent")]
        public string UserAgent { get; set; }  // optional
    }
}

