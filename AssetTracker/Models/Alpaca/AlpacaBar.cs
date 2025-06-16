using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Alpaca
{
    /// <summary>
    /// Represents a single bar (OHLCV) of historical market data from Alpaca.
    /// </summary>
    public class AlpacaBar
    {
        /// <summary>
        /// Gets or sets the timestamp of the bar.
        /// </summary>
        [JsonPropertyName ("t")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the opening price of the bar.
        /// </summary>
        [JsonPropertyName ("o")]
        public decimal Open { get; set; }

        /// <summary>
        /// Gets or sets the highest price during the bar interval.
        /// </summary>
        [JsonPropertyName ("h")]
        public decimal High { get; set; }
        //public decimal h { get; set; }

        /// <summary>
        /// Gets or sets the lowest price during the bar interval.
        /// </summary>
        [JsonPropertyName("l")]
        public decimal Low { get; set; }
        //public decimal L { get; set; }


        /// <summary>
        /// Gets or sets the closing price of the bar.
        /// </summary>
        [JsonPropertyName ("c")]
        public decimal Close { get; set; }

        /// <summary>
        /// Gets or sets the total volume traded during the bar interval.
        /// </summary>
        [JsonPropertyName("v")]
        public long Volume { get; set; }

        /// <summary>
        /// Gets or sets the number of trades that occurred during the bar interval.
        /// </summary>
        [JsonPropertyName("n")]
        public int NumberOfTrades { get; set; }

        /// <summary>
        /// Gets or sets the volume-weighted average price for the bar.
        /// </summary>
        [JsonPropertyName("vw")]
        public decimal VolumeWeightedAverage { get; set; }
    }
}