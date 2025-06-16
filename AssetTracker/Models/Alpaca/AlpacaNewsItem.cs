using System;
using System.Collections.Generic;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a news item retrieved from the Alpaca news feed.
    /// </summary>
    public class AlpacaNewsItem
    {
        /// <summary>
        /// Gets or sets the unique identifier of the news item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the author of the news article.
        /// </summary>
        public string? Author { get; set; } 

        /// <summary>
        /// Gets or sets the headline of the news article.
        /// </summary>
        public string? Headline { get; set; } 

        /// <summary>
        /// Gets or sets a short summary of the news article.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Gets or sets the URL to the full news article.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Gets or sets the source (e.g., news agency or publication) of the article.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets the full content of the news article, if available.
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the article was created or published.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the list of stock symbols mentioned in the article.
        /// </summary>
        public List<string>? Symbols { get; set; }
    }
}