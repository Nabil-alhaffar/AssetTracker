using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a user's watchlist containing a collection of stock symbols.
    /// </summary>
    public class Watchlist
    {
        /// <summary>
        /// MongoDB internal identifier.
        /// </summary>
        [BsonId]  // MongoDB will map _id to this property
        public ObjectId MongoId { get; set; } // MongoDB uses ObjectId by default

        /// <summary>
        /// The user ID this watchlist belongs to.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Unique identifier of the watchlist.
        /// </summary>
        [Required]
        [BsonRepresentation(BsonType.String)]
        public Guid WatchlistId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Name of the watchlist, e.g., "Tech Stocks".
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// List of stock symbols included in the watchlist.
        /// </summary>
        public List<string> Symbols { get; set; } = new List<string>();

        //public ICollection<Stock> Stocks { get; set; } = new List<Stock>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Watchlist()
        {
        }
    }
}