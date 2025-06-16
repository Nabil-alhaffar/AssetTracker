using System;
using System.Collections.Generic;
using Alpaca.Markets;

namespace AssetTracker.Models.Alpaca
{
    /// <summary>
    /// Represents a response object containing a collection of AlpacaBar entries.
    /// </summary>
    public class AlpacaBarsResponse
    {
        /// <summary>
        /// Gets or sets the list of historical bar data entries.
        /// Each entry contains OHLCV data for a specific time interval.
        /// </summary>
        public List<AlpacaBar> bars { get; set; }
    }
}