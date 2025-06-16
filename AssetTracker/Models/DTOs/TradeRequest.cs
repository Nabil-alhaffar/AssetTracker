using System;
using AssetTracker.Models.Enums;
namespace AssetTracker.Models
{
	public sealed record TradeRequest
	{
		public string Symbol { get; set; } = null!;
        public decimal Quantity { get; set; }
		public OrderSide Type { get; set; }
	}
}

