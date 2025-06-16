using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a user of the asset tracking system.
    /// </summary>
    public sealed record User
    {
        /// <summary>
        /// MongoDB internal identifier.
        /// </summary>
        [BsonId]  // MongoDB will map _id to this property
        public ObjectId MongoId { get; set; } // MongoDB uses ObjectId by default

        /// <summary>
        /// Unique identifier for the user (GUID).
        /// </summary>
        [Required]
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        /// <summary>
        /// User's first name.
        /// </summary>
        [Required]
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// User's last name.
        /// </summary>
        [Required]
        public string LastName { get; set; } = null!;

        /// <summary>
        /// User's email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Username for login or display.
        /// </summary>
        [Required]
        public string UserName { get; set; } = null!;

        // /// <summary>
        // /// The user's portfolio.
        // /// </summary>
        // public Portfolio Portfolio { get; set; } = new Portfolio();

        // /// <summary>
        // /// User's watchlists.
        // /// </summary>
        // public List<Watchlist> Watchlists { get; set; } = new List<Watchlist>();

        /// <summary>
        /// Hashed password for secure authentication.
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = null!;

        /// <summary>
        /// Salt used in hashing the password.
        /// </summary>
        [Required]
        public string PasswordSalt { get; set; } = null!;

        /// <summary>
        /// Refresh token for maintaining authentication session.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Expiration time of the refresh token.
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; set; }

        /// <summary>
        /// User's time zone identifier, defaults to UTC.
        /// </summary>
        public string? TimeZoneId { get; set; } = "UTC";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public User()
        {
            //Portfolio.UserId = this.UserId;
        }

        /// <summary>
        /// Parameterized constructor to initialize a user.
        /// </summary>
        /// <param name="firstName">First name of user.</param>
        /// <param name="lastName">Last name of user.</param>
        /// <param name="userId">User's unique GUID.</param>
        /// <param name="email">Email address.</param>
        /// <param name="passwordHash">Hashed password.</param>
        /// <param name="passwordSalt">Password salt.</param>
        public User(string firstName, string lastName, Guid userId, string email, string passwordHash, string passwordSalt)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.UserId = userId;
            this.Email = email;
            this.PasswordHash = passwordHash;
            this.PasswordSalt = passwordSalt;

            //Portfolio = new Portfolio();
            //Watchlists = new List<Watchlist>();
        }
    }
}