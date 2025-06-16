using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a trading position within a user's portfolio.
    /// </summary>
    public sealed record Position
    {
        /// <summary>
        /// Gets or sets the ID of the user who owns this position.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the position.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid PositionId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the quantity of shares/contracts held.
        /// </summary>
        [Required]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the average purchase price per unit.
        /// </summary>
        [Required]
        public decimal AveragePurchasePrice { get; set; }

        /// <summary>
        /// Gets or sets the symbol of the stock or asset.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the position type (e.g., Long, Short).
        /// </summary>
        public PositionType Type { get; set; }

        /// <summary>
        /// Gets the ratio of this position's market value relative to the total portfolio value.
        /// </summary>
        public decimal PositionRatio { get; private set; }

        /// <summary>
        /// Gets or sets the current market price of the asset.
        /// </summary>
        public decimal CurrentPrice { get; set; } // Store real-time price here

        /// <summary>
        /// Gets the total market value of this position (Quantity * CurrentPrice).
        /// </summary>
        public decimal MarketValue => Quantity * CurrentPrice;

        /// <summary>
        /// Gets the total cost basis of this position (AveragePurchasePrice * Quantity).
        /// </summary>
        public decimal TotalCost => AveragePurchasePrice * Quantity;

        /// <summary>
        /// Computes the position ratio relative to the total portfolio value.
        /// </summary>
        /// <param name="totalPortfolioValue">The total market value of the portfolio.</param>
        public void ComputePositionRatio(decimal totalPortfolioValue)
        {
            PositionRatio = totalPortfolioValue > 0 ? Math.Abs(MarketValue) / totalPortfolioValue : 0;
        }

        public Position()
        {
        }
    }
}