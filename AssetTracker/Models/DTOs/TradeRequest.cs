using System;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a request to place a trade order for a specific stock symbol.
    /// </summary>
    public sealed record TradeRequest
    {
        /// <summary>
        /// The stock symbol to trade (e.g., "AAPL").
        /// </summary>
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// The quantity of shares to trade.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// The side of the order: Buy or Sell.
        /// </summary>
        public TradeSide Side { get; set; }

        /// <summary>
        /// The intent of the order: (To close or open a position)
        /// </summary>
        public TradeIntent Intent { get; set; }

    }
}