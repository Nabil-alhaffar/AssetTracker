using System;
namespace AssetTracker.Models.Alpaca
{
	public class AlpacaBar
	{
        public DateTime t { get; set; }
        public decimal o { get; set; }
        public decimal h { get; set; }
        public decimal l { get; set; }
        public decimal c { get; set; }
        public long v { get; set; }
        public int n { get; set; }
        public decimal vw { get; set; }
    }
}

