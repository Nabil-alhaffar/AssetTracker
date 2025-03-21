using System;
namespace AssetTracker.Models
{
	public sealed record PnL
	{
		public decimal PNLValue { get; set; }
		public decimal PNLPercentage { get; set; }
	}
}

