using System;
namespace AssetTracker.Models.Finnhub
{
	public class Quote
	{

        public decimal C { get; set; }  // Current price
        public decimal H { get; set; }  // High
        public decimal L { get; set; }  // Low
        public decimal O { get; set; }  // Open
        public decimal Pc { get; set; } // Previous close
        public decimal T { get; set; }  // Timestamp

    }
}

