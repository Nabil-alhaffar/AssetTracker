using System;
namespace AssetTracker.Models
{
	public class TradeRequest
	{
		public string Symbol { get; set; }
		public decimal Quantity { get; set; }
		public OrderType Type { get; set; }
	}
}

