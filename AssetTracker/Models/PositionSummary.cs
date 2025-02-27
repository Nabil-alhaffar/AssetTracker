﻿using System;
namespace AssetTracker.Models
{
    public sealed record PositionSummary
    {
        public Guid PositionId { get; set; }
        public string Symbol { get; set; }
        public decimal Quantity { get; set; }
        public decimal AveragePurchasePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal MarketValue => CurrentPrice * Quantity;
        public decimal TotalCost => AveragePurchasePrice * Quantity;
        public decimal PNL => MarketValue - TotalCost; // Profit or Loss
    }
}

