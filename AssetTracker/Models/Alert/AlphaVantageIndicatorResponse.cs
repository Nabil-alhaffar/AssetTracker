using System;
using Newtonsoft.Json;

namespace AssetTracker.Models
{
    public class AlphaVantageIndicatorResponse
    {
        [JsonProperty("Time Series (1min)")]
        public Dictionary<string, TimeSeriesData> TimeSeries { get; set; }

        [JsonProperty("Technical Analysis: SMA")]
        public Dictionary<string, IndicatorData> SMA { get; set; }

        [JsonProperty("Technical Analysis: EMA")]
        public Dictionary<string, IndicatorData> EMA { get; set; }

        [JsonProperty("Technical Analysis: MACD")]
        public Dictionary<string, MACDData> MACD { get; set; }

        [JsonProperty("Technical Analysis: RSI")]
        public Dictionary<string, IndicatorData> RSI { get; set; }

        [JsonProperty("Technical Analysis: BBANDS")]
        public Dictionary<string, BollingerBandData> BBANDS { get; set; }

        public decimal? IndicatorValue => SMA?.Values.FirstOrDefault()?.Value ??
                                          EMA?.Values.FirstOrDefault()?.Value ??
                                          MACD?.Values.FirstOrDefault()?.MACD_Signal ??
                                          RSI?.Values.FirstOrDefault()?.Value;
    }

    public class TimeSeriesData
    {
        [JsonProperty("4. close")]
        public decimal? Close { get; set; }
    }

    public class IndicatorData
    {
        [JsonProperty("SMA")]
        public decimal? SMA { get; set; }

        [JsonProperty("RSI")]
        public decimal? RSI { get; set; }
        [JsonProperty("EMA")]
        public decimal? EMA { get; set; }

        public decimal? Value => SMA ?? RSI ?? EMA; // Automatically pick the correct one
    }

    public class BollingerBandData
    {
        [JsonProperty("Real Upper Band")]
        public decimal? UpperBand { get; set; }

        [JsonProperty("Real Middle Band")]
        public decimal? MiddleBand { get; set; }

        [JsonProperty("Real Lower Band")]
        public decimal? LowerBand { get; set; }
    }

    public class MACDData
    {
        [JsonProperty("MACD")]
        public decimal? MACD { get; set; }

        [JsonProperty("MACD_Signal")]
        public decimal? MACD_Signal { get; set; }

        [JsonProperty("MACD_Hist")]
        public decimal? MACD_Hist { get; set; }
    }

}

