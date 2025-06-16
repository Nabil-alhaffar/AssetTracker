using System;
using AssetTracker.Services;
using Newtonsoft.Json;

namespace AssetTracker.Models.AlphaVantage
{
    public class AlphaVantageTimeSeries
    {
        [JsonProperty("Time Series (5min)")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry> TimeSeries5Min { get; set; }

        [JsonProperty("Time Series (15min)")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry> TimeSeries15Min { get; set; }

        [JsonProperty("Time Series (30min)")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry> TimeSeries30Min { get; set; }

        [JsonProperty("Time Series (60min)")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry> TimeSeries60Min { get; set; }

        [JsonProperty("Time Series (Daily)")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry> DailyTimeSeries { get; set; }

        [JsonProperty("Weekly Time Series")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry> WeeklyTimeSeries { get; set; }

        [JsonProperty("Monthly Time Series")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry> MonthlyTimeSeries { get; set; }

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

