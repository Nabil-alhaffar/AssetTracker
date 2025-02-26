using System;
namespace AssetTracker.Models
{
	public sealed record HistoricalData
	{
        
        public DateTime Date { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public decimal Open { get; set; }
        public double Volume { get; set; }
        
        public HistoricalData()
		{
		}
	}
}

