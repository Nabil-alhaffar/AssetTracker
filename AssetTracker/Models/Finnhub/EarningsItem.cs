using System;
namespace AssetTracker.Models.Finnhub
{
    public class EarningsItem
    {
        public string Symbol { get; set; }
        public string Date { get; set; }
        public string Hour { get; set; }
        public decimal? Estimate { get; set; }
        public decimal? Actual { get; set; }
    }

}