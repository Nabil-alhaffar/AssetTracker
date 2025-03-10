using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
	public class Watchlist
	{
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
		[Required]
		[BsonRepresentation(BsonType.String)]
		public Guid WatchlistId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } // e.g., "Tech Stocks"
        public List<string> Symbols { get; set; } = new List<string>(); // Stock symbols
        //public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
		public Watchlist()
		{
		}
	}
}

