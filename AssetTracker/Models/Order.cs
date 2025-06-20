using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a trade order placed by a user.
    /// </summary>
    public sealed record Order
    {
        /// <summary>
        /// Gets or sets the MongoDB ObjectId.
        /// </summary>
        [BsonId]
        public ObjectId MongoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the order.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid OrderId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the unique identifier of the user placing the order.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the stock symbol for which the order is placed.
        /// </summary>
        [Required]
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// Gets or sets the quantity of shares in the order.
        /// </summary>
        [Required]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price per share for the order.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the side of the order (Buy, Sell).
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public TradeSide Side { get; set; }


        /// <summary>
        /// Gets or sets the int of the order (Buy to open/close, Sell to open/close ).
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public TradeIntent Intent { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the order was placed.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}