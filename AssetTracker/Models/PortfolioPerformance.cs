using System;
namespace AssetTracker.Models
{
	public sealed record PortfolioPerformance
	{
        public decimal PNL { get; set; }
        public decimal ReturnPercentage { get; set; }
        
    }
}

