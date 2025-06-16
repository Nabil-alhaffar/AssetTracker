using System;
using System.Collections.Generic;

namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents social sentiment data for a symbol from Reddit and Twitter.
    /// </summary>
    public class SocialSentiment
    {
        /// <summary>
        /// Gets or sets the stock symbol for which sentiment data is collected.
        /// </summary>
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// Gets or sets the list of sentiment items collected from Reddit.
        /// </summary>
        public List<SentimentItem> Reddit { get; set; } = null!;

        /// <summary>
        /// Gets or sets the list of sentiment items collected from Twitter.
        /// </summary>
        public List<SentimentItem> Twitter { get; set; } = null!;
    }
}