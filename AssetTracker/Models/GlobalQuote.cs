using System;
namespace AssetTracker.Models
{

    public sealed record GlobalQuote
    {
        public string Symbol { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal LastPrice { get; set; }
        public long Volume { get; set; }
        public string LatestTradingDay { get; set; }
        public decimal PreviousClose { get; set; }
        public decimal Change { get; set; }
        public string ChangePercent { get; set; }
    }


}

