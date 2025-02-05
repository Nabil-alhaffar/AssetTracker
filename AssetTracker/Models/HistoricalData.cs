using System;
namespace AssetTracker.Models
{
	public class HistoricalData
	{
        
        public DateTime Date { get; set; }
        public double ClosePrice { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Open { get; set; }
        
        public HistoricalData()
		{
		}
	}
}

