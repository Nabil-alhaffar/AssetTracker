using System;
namespace AssetTracker.Models.Finnhub
{
    public class RecommendationTrend
    {
        public string Symbol { get; set; }
        public string Period { get; set; }
        public int StrongBuy { get; set; }
        public int Buy { get; set; }
        public int Hold { get; set; }
        public int Sell { get; set; }
        public int StrongSell { get; set; }
    }
}

