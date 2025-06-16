using System;
using System.Collections.Generic;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the response containing a list of Alpaca news items.
    /// </summary>
    public class AlpacaNewsResponse
    {
        /// <summary>
        /// Gets or sets the list of news items retrieved from Alpaca.
        /// </summary>
        public List<AlpacaNewsItem>? News { get; set; }
    }
}