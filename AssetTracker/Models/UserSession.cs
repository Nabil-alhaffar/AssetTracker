using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a user session for tracking login activity and session state.
    /// </summary>
    public sealed record UserSession
    {
        /// <summary>
        /// MongoDB internal identifier.
        /// </summary>
        [BsonId]  // MongoDB will map _id to this property
        public ObjectId Id { get; set; }

        /// <summary>
        /// The unique identifier of the user this session belongs to.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Unique identifier for the session.
        /// </summary>
        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; } = null!;

        /// <summary>
        /// Timestamp when the session was created.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when the session ended, if applicable.
        /// </summary>
        [JsonPropertyName("EndedAt")]
        public DateTime? EndedAt { get; set; }

        /// <summary>
        /// IP address from which the session was created.
        /// </summary>
        public string IpAddress { get; set; } = null!;

        /// <summary>
        /// User agent string identifying the client software.
        /// </summary>
        [JsonPropertyName("userAgent")]
        public string UserAgent { get; set; } = null!;
    }
}