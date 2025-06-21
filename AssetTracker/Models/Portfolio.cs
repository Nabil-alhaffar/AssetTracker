using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a user's investment portfolio, including positions, margin info, and available funds.
    /// </summary>
    public sealed record Portfolio
    {
        /// <summary>
        /// Gets or sets the MongoDB ObjectId used as the primary key.
        /// </summary>
        [BsonId]
        public ObjectId MongoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who owns the portfolio.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the collection of positions keyed by stock symbol.
        /// </summary>
        public Dictionary<string, Position> Positions { get; set; } = new();

        /// <summary>
        /// Gets or sets the amount of funds available for trading (cash).
        /// </summary>
        public decimal AvailableFunds { get; set; } = 0;

        /// <summary>
        /// Gets or sets the current margin used by open positions.
        /// </summary>
        public decimal MarginUsed { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum margin limit (initial buying power extension).
        /// </summary>
        public decimal MarginLimit { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maintenance margin requirement as a decimal (e.g., 0.25 = 25%).
        /// </summary>
        public decimal MaintenanceMarginRequirement { get; set; } = 0.25m;


        /// <summary>
        /// Gets or sets the initial margin requirement as a decimal (e.g., 0.25 = 25%).
        /// </summary>
        public decimal InitialMarginRequirement { get; set; } = 0.50m;


        /// <summary>
        /// The total value of long positions.
        /// </summary>
        [BsonIgnore]
        public decimal LongMarketValue =>
            Positions.Where(p => p.Value.Type == PositionType.Long)
                     .Sum(p => p.Value.CurrentPrice * p.Value.Quantity);

        /// <summary>
        /// The total liability from short positions.
        /// </summary>
        [BsonIgnore]
        public decimal ShortMarketValue =>
            Positions.Where(p => p.Value.Type == PositionType.Short)
                     .Sum(p => p.Value.CurrentPrice * Math.Abs(p.Value.Quantity));

        /// <summary>
        /// The user's equity: cash + long - short.
        /// </summary>
        [BsonIgnore]
        public decimal Equity => AvailableFunds + LongMarketValue - ShortMarketValue;

        /// <summary>
        /// Available buying power: includes unused margin limit.
        /// </summary>
        [BsonIgnore]
        public decimal BuyingPower => AvailableFunds + (MarginLimit - MarginUsed);

        /// <summary>
        /// Whether the user is in a margin call (equity below maintenance threshold).
        /// </summary>
        [BsonIgnore]
        public bool IsInMarginCall => Equity < (MarginUsed / (1 - MaintenanceMarginRequirement));

        /// <summary>
        /// Initializes a new instance of the <see cref="Portfolio"/> class.
        /// </summary>
        public Portfolio()
        {
            Positions = new Dictionary<string, Position>();
        }
    }
}
