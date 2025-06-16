using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the response for the most active stocks from Alpaca,
    /// including a list of active items and the timestamp of the last update.
    /// </summary>
    public class AlpacaMostActiveResponse
    {
        /// <summary>
        /// Gets or sets the timestamp when the data was last updated.
        /// </summary>
        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the list of most actively traded stocks.
        /// </summary>
        [JsonPropertyName("most_actives")]
        public List<AlpacaMostActiveItem> MostActives { get; set; }
    }
}