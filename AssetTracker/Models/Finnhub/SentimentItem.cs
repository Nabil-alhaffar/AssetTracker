using System;

namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents a sentiment data point at a specific time with mention counts and sentiment score.
    /// </summary>
    public class SentimentItem
    {
        /// <summary>
        /// Gets or sets the timestamp of the sentiment data.
        /// </summary>
        public string AtTime { get; set; }

        /// <summary>
        /// Gets or sets the total number of mentions.
        /// </summary>
        public int Mention { get; set; }

        /// <summary>
        /// Gets or sets the number of positive mentions.
        /// </summary>
        public int PositiveMention { get; set; }

        /// <summary>
        /// Gets or sets the number of negative mentions.
        /// </summary>
        public int NegativeMention { get; set; }

        /// <summary>
        /// Gets or sets the sentiment score.
        /// </summary>
        public decimal Score { get; set; }
    }
}