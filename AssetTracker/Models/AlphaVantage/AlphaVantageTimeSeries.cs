using System;
using System.Collections.Generic;
using AssetTracker.Services;
using Newtonsoft.Json;

namespace AssetTracker.Models.AlphaVantage
{
    /// <summary>
    /// Represents various time series data retrieved from Alpha Vantage API,
    /// including intraday, daily, weekly, and monthly stock price series.
    /// </summary>
    public class AlphaVantageTimeSeries
    {
        /// <summary>
        /// Gets or sets the 5-minute interval time series data.
        /// Key is a timestamp string, value is the corresponding data entry.
        /// </summary>
        [JsonProperty("Time Series (5min)")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry>? TimeSeries5Min { get; set; }

        /// <summary>
        /// Gets or sets the 15-minute interval time series data.
        /// </summary>
        [JsonProperty("Time Series (15min)")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry>? TimeSeries15Min { get; set; }

        /// <summary>
        /// Gets or sets the 30-minute interval time series data.
        /// </summary>
        [JsonProperty("Time Series (30min)")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry>? TimeSeries30Min { get; set; }

        /// <summary>
        /// Gets or sets the 60-minute interval time series data.
        /// </summary>
        [JsonProperty("Time Series (60min)")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry>? TimeSeries60Min { get; set; }

        /// <summary>
        /// Gets or sets the daily time series data.
        /// </summary>
        [JsonProperty("Time Series (Daily)")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry>? DailyTimeSeries { get; set; }

        /// <summary>
        /// Gets or sets the weekly time series data.
        /// </summary>
        [JsonProperty("Weekly Time Series")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry>? WeeklyTimeSeries { get; set; }

        /// <summary>
        /// Gets or sets the monthly time series data.
        /// </summary>
        [JsonProperty("Monthly Time Series")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry>? MonthlyTimeSeries { get; set; }

        /// <summary>
        /// Returns the appropriate time series dictionary based on the function name.
        /// Defaults to returning intraday time series in order of availability if no match.
        /// </summary>
        /// <param name="function">The Alpha Vantage time series function name, e.g. "TIME_SERIES_DAILY".</param>
        /// <returns>
        /// A dictionary mapping timestamp strings to <see cref="AlphaVantageTimeSeriesEntry"/> objects
        /// for the specified time series function.
        /// </returns>
        public Dictionary<string, AlphaVantageTimeSeriesEntry> GetTimeSeries(string function)
        {
            return function switch
            {
                "TIME_SERIES_DAILY" => DailyTimeSeries,
                "TIME_SERIES_WEEKLY" => WeeklyTimeSeries,
                "TIME_SERIES_MONTHLY" => MonthlyTimeSeries,
                _ => TimeSeries5Min ?? TimeSeries15Min ?? TimeSeries30Min ?? TimeSeries60Min
            };
        }
    }
}