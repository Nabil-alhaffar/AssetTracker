using System;
using System.Collections.Generic;

namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents the response containing a list of earnings calendar items, as provided by Finnhub.
    /// </summary>
    public sealed record EarningsCalendarResponse
    {
        /// <summary>
        /// List of earnings calendar items.
        /// </summary>
        public List<EarningsItem> EarningsCalendar { get; set; } = new List<EarningsItem>();
    }
}