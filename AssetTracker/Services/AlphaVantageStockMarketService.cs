using System;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using AssetTracker.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.Extensions.Caching.Distributed;
using AssetTracker.Services.Interfaces;
using AssetTracker.Models.Enums;
using AssetTracker.Models.AlphaVantage;
using AssetTracker.Models.Alert;

namespace AssetTracker.Services
{

    /// <summary>
    /// Interface for interacting with the Alpha Vantage stock market data API.
    /// </summary>
    public class AlphaVantageStockMarketService: IAlphaVantageStockMarketService
	{
        private readonly HttpClient _httpClient;
        private readonly string APIKey;
        private readonly IDistributedCache _cache;
        private const string BaseURL = "https://www.alphavantage.co/query";

        public AlphaVantageStockMarketService(HttpClient httpClient, IDistributedCache cache, IConfiguration configuration)

		{

            _httpClient = httpClient;
            _cache = cache;
            APIKey = configuration["AlphaVantage:ApiKey"]; // Fetch API Key from appsettings.json

        }

        /// <summary>
        /// Retrieves the latest stock price for a given symbol from Alpha Vantage or cache.
        /// </summary>
        /// <param name="symbol">The stock symbol to query.</param>
        /// <returns>The latest price as a decimal.</returns>
        public async Task<decimal> GetStockPriceAsync(string symbol)
        {
            var cacheKey = $"StockLastPrice:{symbol}";
            var cachedPrice = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedPrice))
            {
                return decimal.Parse(cachedPrice);
            }

            var url = $"{BaseURL}?function=GLOBAL_QUOTE&symbol={symbol}&apikey={APIKey}";
            var response = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);

            var quote = json["Global Quote"];
            if (quote == null)
            {
                return 0; // Handle missing data
            }

            decimal lastPrice = quote["05. price"]?.Value<decimal>() ?? 0;

            await _cache.SetStringAsync(cacheKey, lastPrice.ToString(),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

            return lastPrice;
            //return double.TryParse(closePrice, out double price) ? price : 0;
        }

        /// <summary>
        /// Retrieves the global quote for a given stock symbol, including price, volume, and daily changes.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A <see cref="GlobalQuote"/> object containing quote information.</returns>
        public async Task<GlobalQuote> GetGlobalQuoteAsync(string symbol)
        {
            var cacheKey = $"StockGlobalQuote:{symbol}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonConvert.DeserializeObject<GlobalQuote>(cachedData);
            }

            var url = $"{BaseURL}?function=GLOBAL_QUOTE&symbol={symbol}&apikey={APIKey}";
            var response = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);
            var quote = json["Global Quote"];

            if (quote == null)
            {
                return null; // Handle missing data
            }

            var globalQuote = new GlobalQuote
            {
                Symbol = quote["01. symbol"]?.ToString(),
                Open = quote["02. open"]?.Value<decimal>() ?? 0,
                High = quote["03. high"]?.Value<decimal>() ?? 0,
                Low = quote["04. low"]?.Value<decimal>() ?? 0,
                LastPrice = quote["05. price"]?.Value<decimal>() ?? 0,
                Volume = quote["06. volume"]?.Value<long>() ?? 0,
                LatestTradingDay = quote["07. latest trading day"]?.ToString(),
                PreviousClose = quote["08. previous close"]?.Value<decimal>() ?? 0,
                Change = quote["09. change"]?.Value<decimal>() ?? 0,
                ChangePercent = quote["10. change percent"]?.ToString()
            };

            await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(globalQuote),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

            return globalQuote;
        }


        /// <summary>
        /// Fetches and caches technical indicators for a stock (e.g. RSI, BBANDS, SMA, EMA).
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <param name="requestedIndicators">A list of indicator names (e.g. "RSI", "BBANDS").</param>
        /// <param name="interval">Interval for the indicators (e.g. "daily").</param>
        /// <param name="timePeriod">Time period for the indicators (default is 14).</param>
        /// <param name="limit">The number of most recent entries to return.</param>
        /// <returns>A dictionary of indicator data grouped by date.</returns>
        public async Task<Dictionary<string, Dictionary<string, object>>> GetStockIndicatorsAsync (string symbol, List<string> requestedIndicators, string interval = "daily", int timePeriod = 14, int limit = 100)
        {
            var cacheKey = $"StockIndicators:{symbol}:{interval}:{timePeriod}";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            var allResults = new Dictionary<string, Dictionary<string, object>>();

            if (!string.IsNullOrEmpty(cachedData))
            {
                allResults = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(cachedData);

                // ✅ Convert BBANDS JSON back into a dictionary
                foreach (var entry in allResults.Keys.ToList())
                {
                    if (allResults[entry].ContainsKey("BBANDS"))
                    {
                        allResults[entry]["BBANDS"] = JsonConvert.DeserializeObject<Dictionary<string, decimal?>>(
                            allResults[entry]["BBANDS"].ToString()
                        );
                    }
                }
            }

            var missingIndicators = requestedIndicators
                .Where(indicator => !allResults.Values.Any(data => data.ContainsKey(indicator)))
                .ToList();

            if (missingIndicators.Count > 0)
            {
                var tasks = missingIndicators.Select(async indicator =>
                {
                    string seriesType = "close";
                    string url = $"{BaseURL}?function={indicator}&symbol={symbol}&interval={interval}&time_period={timePeriod}&apikey={APIKey}";

                    if (indicator is "RSI" or "MACD" or "BBANDS" or "SMA" or "EMA")
                    {
                        url += $"&series_type={seriesType}";
                    }

                    Console.WriteLine($"Requesting Indicator: {indicator} - URL: {url}");

                    try
                    {
                        var response = await _httpClient.GetStringAsync(url);
                        Console.WriteLine($"Response for {indicator}: {response}");

                        var data = JsonConvert.DeserializeObject<AlphaVantageIndicatorResponse>(response);
                        if (data == null) return;

                        lock (allResults)
                        {
                            if (indicator == "RSI" && data.RSI != null)
                            {
                                foreach (var entry in data.RSI)
                                {
                                    if (!entry.Value.Value.HasValue) continue;
                                    if (!allResults.ContainsKey(entry.Key)) allResults[entry.Key] = new Dictionary<string, object>();
                                    allResults[entry.Key]["RSI"] = entry.Value.Value;
                                }
                            }
                            else if (indicator == "BBANDS" && data.BBANDS != null)
                            {
                                foreach (var entry in data.BBANDS)
                                {
                                    if (!allResults.ContainsKey(entry.Key))
                                        allResults[entry.Key] = new Dictionary<string, object>();

                                    // ✅ Convert BBANDS to JSON before storing in Redis
                                    allResults[entry.Key]["BBANDS"] = new Dictionary<string, decimal?>
                                                                      {
                                                                            { "UpperBand", entry.Value.UpperBand },
                                                                            { "MiddleBand", entry.Value.MiddleBand },
                                                                            { "LowerBand", entry.Value.LowerBand }
                                                                        };
                                }
                            }
                            else if (indicator == "EMA" && data.EMA != null)
                            {
                                foreach (var entry in data.EMA)
                                {
                                    if (!entry.Value.Value.HasValue) continue;
                                    if (!allResults.ContainsKey(entry.Key)) allResults[entry.Key] = new Dictionary<string, object>();
                                    allResults[entry.Key]["EMA"] = entry.Value.Value;
                                }
                            }
                            else if (indicator == "SMA" && data.SMA != null)
                            {
                                foreach (var entry in data.SMA)
                                {
                                    if (!entry.Value.Value.HasValue) continue;
                                    if (!allResults.ContainsKey(entry.Key)) allResults[entry.Key] = new Dictionary<string, object>();
                                    allResults[entry.Key]["SMA"] = entry.Value.Value;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error fetching {indicator}: {ex.Message}");
                    }
                }).ToList();

                await Task.WhenAll(tasks);
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(allResults), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            // ✅ Apply .Take(limit) when returning data (not when caching)
            return allResults
                .Where(entry => entry.Value.Keys.Any(requestedIndicators.Contains))
                .OrderByDescending(entry => entry.Key) // Ensure most recent data first
                .Take(limit) // Apply limit here
                .ToDictionary(entry => entry.Key, entry => entry.Value
                    .Where(kv => requestedIndicators.Contains(kv.Key))
                    .ToDictionary(kv => kv.Key, kv => kv.Value));
        }

        /// <summary>
        /// Retrieves historical OHLCV stock data for a given symbol and interval.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <param name="interval">The time interval ("daily", "weekly", "monthly", or intraday like "5min").</param>
        /// <returns>A list of <see cref="HistoricalData"/> entries.</returns>
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

        /// <summary>
        /// Fetches the company profile, current price, 52-week data, EPS, sector, and quote info for a stock.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A <see cref="Stock"/> object with detailed overview data.</returns>
        public async Task<Stock> GetStockOverviewAsync(string symbol)
        {
            var cacheKey = $"StockOverview:{symbol}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonConvert.DeserializeObject<Stock>(cachedData);
            }
            try
            {
                //var url = $"{BaseURL}?function=TIME_SERIES_INTRADAY&symbol={symbol}&interval=1min&apikey={APIKey}";
                //var response = await _httpClient.GetStringAsync(url);
                //var json = JObject.Parse(response);
                //Console.WriteLine($"Response: {response}");

                //var timeSeries = json["Time Series (1min)"];
                //var latestTime = timeSeries?.First?.First;
                decimal currentPrice = (decimal)await GetStockPriceAsync(symbol);
                //var currentPrice = (float)latestTime?["4. close"];

                string infoUrl = $"{BaseURL}?function=OVERVIEW&symbol={symbol}&apikey={APIKey}";
                HttpResponseMessage infoResponse = await _httpClient.GetAsync(infoUrl);
                infoResponse.EnsureSuccessStatusCode();

                string jsonResponse = await infoResponse.Content.ReadAsStringAsync();
                var stockInfo = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                var stock = new Stock
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
                    OfficialSite = stockInfo["OfficialSite"] ?? "Unknown",
                    Quote = await GetGlobalQuoteAsync(symbol)
                };
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(stock), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) // Cache expires in 1 hour
                });

                return stock;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching stock overview: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Constructs a Clearbit logo URL for a company's website.
        /// </summary>
        /// <param name="website">The official website of the company.</param>
        /// <returns>A URL pointing to the company logo hosted on Clearbit.</returns>
        public string GetCompanyLogoUrl(string website)
        {
            string domain = DomainExtractor(website);
            return $"https://logo.clearbit.com/{domain}";
        }


        /// <summary>
        /// Extracts the domain from a URL string, stripping "www." if present.
        /// </summary>
        /// <param name="url">The full website URL.</param>
        /// <returns>The domain portion of the URL.</returns>
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



        /// <summary>
        /// Parses a sector string into a <see cref="Sector"/> enum.
        /// </summary>
        /// <param name="sector">The sector name as a string.</param>
        /// <returns>The matching <see cref="Sector"/> enum value, or InformationTechnology by default.</returns>
        private Sector ParseSector(string sector)
        {
            return sector?.ToLower() switch
            {
                "energy" => Sector.Energy,
                "materials" => Sector.Materials,
                "industrials" => Sector.Industrials,
                "consumer discretionary" => Sector.ConsumerDiscretionary,
                "consumer staples" => Sector.ConsumerStaples,
                "healthcare" => Sector.Healthcare,
                "financials" => Sector.Financials,
                "information technology" => Sector.InformationTechnology,
                "communication services" => Sector.CommunicationServices,
                "utilities" => Sector.Utilities,
                "real estate" => Sector.RealEstate,
                _ => Sector.InformationTechnology
            };
        }
    }

}

