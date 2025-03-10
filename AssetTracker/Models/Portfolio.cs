using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
	public sealed record Portfolio
	{


        //[Required]
        //public Guid PortfolioId { get; set; } = Guid.NewGuid();
        //public User User { get; set; }
        [BsonId]  // MongoDB will map _id to this property
        public ObjectId MongoId { get; set; } // MongoDB uses ObjectId by default
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        public Dictionary<string, Position> Positions { get; set; } = new ();
		public decimal AvailableFunds { get; set; } = 0; 


        public Portfolio()
		{

			Positions = new Dictionary<string, Position>();
			 
		}
	}
}

