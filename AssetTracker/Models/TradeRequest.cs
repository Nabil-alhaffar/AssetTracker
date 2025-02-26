using System;
namespace AssetTracker.Models
{
	public sealed record TradeRequest
	{
		public string Symbol { get; set; }
		public decimal Quantity { get; set; }
		public OrderType Type { get; set; }
	}
}

