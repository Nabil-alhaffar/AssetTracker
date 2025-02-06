﻿using System;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using AssetTracker.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace AssetTracker.Services
{
    public class StockService : IStockService
    {
        private readonly HttpClient _httpClient;
        private readonly string APIKey ;
        private const string BaseURL = "https://www.alphavantage.co/query";

        public StockService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            APIKey = configuration["AlphaVantage:ApiKey"]; // Fetch API Key from appsettings.json
        }

        public string GetCompanyLogoUrl(string website)
        {
            string domain = DomainExtractor(website);
            return $"https://logo.clearbit.com/{domain}";
        }
        public async Task<double> GetStockPriceAsync(string symbol)
        {
            var url = $"{BaseURL}?function=TIME_SERIES_INTRADAY&symbol={symbol}&interval=1min&apikey={APIKey}";
            var response = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);

            var timeSeries = json["Time Series (1min)"];
            if (timeSeries == null || !timeSeries.HasValues)
            {
                return 0; // Return 0 or handle error appropriately
            }

            var latestEntry = timeSeries.First;
            var closePrice = latestEntry.First["4. close"]?.ToString();

            return double.TryParse(closePrice, out double price) ? price : 0;
        }

        public async Task<IEnumerable<HistoricalData>> GetHistoricalDataAsync(string symbol, string interval = "daily")
        {
            var function = interval.ToLower() switch
            {
                "5min" or "15min" or "30min" or "60min" or "240min" => "TIME_SERIES_INTRADAY",
                "daily" => "TIME_SERIES_DAILY",
                "weekly" => "TIME_SERIES_WEEKLY",
                "monthly" => "TIME_SERIES_MONTHLY",
                _ => "TIME_SERIES_DAILY"
            };

            string intervalQuery = (function == "TIME_SERIES_INTRADAY") ? $"&interval={interval}" : "";
            var url = $"{BaseURL}?function={function}{intervalQuery}&symbol={symbol}&apikey={APIKey}";

            Console.WriteLine($"Requesting URL: {url}");
            var response = await _httpClient.GetStringAsync(url);
            Console.WriteLine($"Response: {response}");

            var data = JsonConvert.DeserializeObject<AlphaVantageTimeSeries>(response);
            if (data == null) return null;

            // Use the improved GetTimeSeries method
            Dictionary<string, AlphaVantageTimeSeriesEntry> timeSeries = data.GetTimeSeries(function);

            if (timeSeries == null || !timeSeries.Any()) return null;

            return timeSeries.Select(item => new HistoricalData
            {
                Date = DateTime.Parse(item.Key),
                ClosePrice = item.Value.Close,
                Open = item.Value.Open,
                Low = item.Value.Low,
                High = item.Value.High,
                Volume = item.Value.Volume
            })
            .OrderBy(d => d.Date)
            .ToList();
        }


        public async Task<Stock> GetStockOverviewAsync(string symbol)
        {
            try
            {
                var url = $"{BaseURL}?function=TIME_SERIES_INTRADAY&symbol={symbol}&interval=1min&apikey={APIKey}";
                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);
                Console.WriteLine($"Response: {response}");

                var timeSeries = json["Time Series (1min)"];
                var latestTime = timeSeries?.First?.First;
                var currentPrice = (float)latestTime?["4. close"];

                string infoUrl = $"{BaseURL}?function=OVERVIEW&symbol={symbol}&apikey={APIKey}";
                HttpResponseMessage infoResponse = await _httpClient.GetAsync(infoUrl);
                infoResponse.EnsureSuccessStatusCode();

                string jsonResponse = await infoResponse.Content.ReadAsStringAsync();
                var stockInfo = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                return new Stock
                {
                    Symbol = symbol,
                    CurrentPrice = currentPrice,
                    CompanyName = stockInfo["Name"] ?? symbol,
                    MarketCap = stockInfo["MarketCapitalization"] ?? 0,
                    EPS = stockInfo["EPS"] ?? 0,
                    StockStatus = (Stock.Status)(stockInfo["52WeekHigh"] > currentPrice ? 0 : 1),
                    StockSector = ParseSector(stockInfo["Sector"]?.ToString()),
                    Exchange = stockInfo["Exchange"]?.ToString() ?? "Unknown",
                    High52Week = stockInfo["52WeekHigh"] ?? 0,
                    Low52Week = stockInfo["52WeekLow"] ?? 0,
                    MovingAverage50Day = stockInfo["50DayMovingAverage"] ?? 0,
                    MovingAverage200Day = stockInfo["200DayMovingAverage"] ?? 0,
                    Country = stockInfo["Country"] ?? "Unknown",
                    DividendDate = stockInfo["DividendDate"] ?? DateTimeZoneHandling.Local,
                    ExDividendDate = stockInfo["ExDividendDate"] ?? DateTimeZoneHandling.Local,
                    AnalystTargetPrice = stockInfo["AnalystTargetPrice"] ?? "Unknown",
                    Description = stockInfo["Description"] ?? "Unknown",
                    OfficialSite = stockInfo["OfficialSite"] ?? "Unknown"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching stock overview: {ex.Message}");
                return null;
            }
        }

        private string DomainExtractor(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL cannot be null or empty", nameof(url));

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                    !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    url = "http://" + url;
                }
            }

            try
            {
                Uri uri = new Uri(url);
                string host = uri.Host;
                return host.StartsWith("www.") ? host.Substring(4) : host;
            }
            catch (UriFormatException ex)
            {
                throw new UriFormatException($"Invalid URL '{url}'. Exception: {ex.Message}");
            }
        }

        private Stock.Sector ParseSector(string sector)
        {
            return sector?.ToLower() switch
            {
                "energy" => Stock.Sector.Energy,
                "materials" => Stock.Sector.Materials,
                "industrials" => Stock.Sector.Industrials,
                "consumer discretionary" => Stock.Sector.ConsumerDiscretionary,
                "consumer staples" => Stock.Sector.ConsumerStaples,
                "healthcare" => Stock.Sector.Healthcare,
                "financials" => Stock.Sector.Financials,
                "information technology" => Stock.Sector.InformationTechnology,
                "communication services" => Stock.Sector.CommunicationServices,
                "utilities" => Stock.Sector.Utilities,
                "real estate" => Stock.Sector.RealEstate,
                _ => Stock.Sector.InformationTechnology
            };
        }
    }

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


    public class AlphaVantageTimeSeriesEntry
    {
        [JsonProperty("1. open")]
        public double Open { get; set; }

        [JsonProperty("2. high")]
        public double High { get; set; }

        [JsonProperty("3. low")]
        public double Low { get; set; }

        [JsonProperty("4. close")]
        public double Close { get; set; }

        [JsonProperty("5. volume")]
        public long Volume { get; set; }
    }
}

