using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
    public sealed record User
    {
        [BsonId]  // MongoDB will map _id to this property
        public ObjectId MongoId { get; set; } // MongoDB uses ObjectId by default

        [Required]
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; } = Guid.NewGuid();

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        //public Portfolio Portfolio { get; set; } = new Portfolio();
        public List<Watchlist> Watchlists { get; set; } = new List<Watchlist>();

        // Fields for storing hashed password and salt
        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string PasswordSalt { get; set; }

        public User()
        {
            //Portfolio.UserId = this.UserId;
        }

        public User(string firstName, string lastName, Guid userId, string email, string passwordHash, string passwordSalt)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.UserId = userId;
            this.Email = email;
            this.PasswordHash = passwordHash;
            this.PasswordSalt = passwordSalt;

            //Portfolio = new Portfolio();
            Watchlists = new List<Watchlist>();
        }
    }
}
