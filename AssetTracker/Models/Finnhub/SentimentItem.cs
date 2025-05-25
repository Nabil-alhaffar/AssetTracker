using System;
namespace AssetTracker.Models.Finnhub
{
    public class SentimentItem
    {
        public string AtTime { get; set; }
        public int Mention { get; set; }
        public int PositiveMention { get; set; }
        public int NegativeMention { get; set; }
        public decimal Score { get; set; }
    }
}

