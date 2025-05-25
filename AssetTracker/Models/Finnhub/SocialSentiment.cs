using System;
namespace AssetTracker.Models.Finnhub
{
    public class SocialSentiment
    {
        public string Symbol { get; set; }
        public List<SentimentItem> Reddit { get; set; }
        public List<SentimentItem> Twitter { get; set; }
    }
}

