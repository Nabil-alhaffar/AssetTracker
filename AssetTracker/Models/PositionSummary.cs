using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a summarized view of a trading position, including current market data and PnL.
    /// </summary>
    public sealed record PositionSummary
    {
        /// <summary>
        /// Unique identifier of the position.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid PositionId { get; set; }

        /// <summary>
        /// The stock or asset symbol.
        /// </summary>
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// Quantity of shares or units held.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Average price paid per share or unit.
        /// </summary>
        public decimal AveragePurchasePrice { get; set; }

        /// <summary>
        /// Current market price per share or unit.
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// Total market value of the position (CurrentPrice * Quantity).
        /// </summary>
        public decimal MarketValue => CurrentPrice * Quantity;

        /// <summary>
        /// Total cost basis of the position (AveragePurchasePrice * Quantity).
        /// </summary>
        public decimal TotalCost => AveragePurchasePrice * Quantity;

        /// <summary>
        /// Open Profit or Loss in absolute terms.
        /// </summary>
        public decimal OpenPNL { get; set; }

        /// <summary>
        /// Open Profit or Loss as a percentage.
        /// </summary>
        public decimal OpenPNLPercentage { get; set; }
    }
}