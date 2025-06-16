using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
//using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AssetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using AssetTracker.Models;
using static System.Net.WebRequestMethods;
using System.Net.Http.Headers;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using AssetTracker.Models.Alpaca;

namespace AssetTracker.Services
{
    public class AlpacaStockMarketService : IAlpacaStockMarketService
    {
        private readonly ILogger<AlpacaStockMarketService> _logger;
        //private readonly IAlpacaDataStreamingClient _dataClient;
        //private readonly IAlpacaTradingClient _tradingClient; // ✅ Add Trading Client
        private readonly List<string> _subscribedSymbols = new();
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly HttpClient _client;
        private List<SymbolLookupResult> _symbolCache = new();
        private readonly SemaphoreSlim _loadLock = new(1, 1);


        /// <summary>
        /// Creates a new instance of <see cref="AlpacaStockMarketService"/>.
        /// Initializes HttpClient with Alpaca API credentials and base URL.
        /// </summary>
        /// <param name="configuration">Configuration to read API keys from.</param>
        /// <param name="logger">Logger instance for logging.</param>
        public AlpacaStockMarketService(IConfiguration configuration, ILogger<AlpacaStockMarketService> logger)
        //IAlpacaDataStreamingClient dataClient, IAlpacaTradingClient tradingClient)
        { 
            _logger = logger;
            //_dataClient = dataClient;
            //_tradingClient = tradingClient;
            _apiKey = configuration["Alpaca:ApiKey"];
            _apiSecret = configuration["Alpaca:ApiSecret"];
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://data.alpaca.markets"),
            };
            _client.DefaultRequestHeaders.Add("APCA-API-KEY-ID", _apiKey);
            _client.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", _apiSecret);
        }

        /// <summary>
        /// Gets batch snapshots for the specified list of symbols from Alpaca.
        /// </summary>
        /// <param name="symbols">List of stock symbols to fetch snapshots for. Cannot be null or empty.</param>
        /// <returns>Raw JSON string containing snapshot data.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="symbols"/> is null or empty.</exception>
        /// <exception cref="Exception">Thrown if the HTTP request to Alpaca API fails.</exception>
        public async Task<string> GetSnapshotsAsync(List<string> symbols)
        {
            if (symbols == null || symbols.Count == 0)
            {
                throw new ArgumentException("Symbols list cannot be null or empty.");
            }

            var symbolQuery = string.Join(",", symbols);
            var url = $"/v2/stocks/snapshots?symbols={symbolQuery}";

            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            else
            {
                throw new Exception($"Batch snapshot fetch failed: {response.StatusCode}");
            }
        }


        /// <summary>
        /// Gets historical bars (OHLCV) data for a given stock symbol.
        /// </summary>
        /// <param name="symbol">Stock symbol to query.</param>
        /// <param name="timeframe">Timeframe for bars (e.g., "1Day"). Defaults to "1Day".</param>
        /// <param name="start">Start date for historical data in ISO format (yyyy-MM-dd). Defaults to "2024-01-01".</param>
        /// <returns>An <see cref="AlpacaBarsResponse"/> containing bar data, or null if request fails.</returns>
        public async Task<AlpacaBarsResponse?> GetHistoricalBarsAsync(string symbol, string timeframe = "1Day", string start = "2024-01-01")
        {
            var url = $"/v2/stocks/{symbol}/bars?start={start}&timeframe={timeframe}";
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            var barsResponse = JsonSerializer.Deserialize<AlpacaBarsResponse>(jsonString);

            return barsResponse;
        }


        /// <summary>
        /// Retrieves recent news articles related to a specific stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol to get news for.</param>
        /// <param name="limit">Maximum number of news items to return. Defaults to 20.</param>
        /// <returns>List of <see cref="AlpacaNewsItem"/> objects.</returns>
        public async Task<List<AlpacaNewsItem>> GetNewsAsync(string symbol, int limit = 20)
        {
            var url = $"/v1beta1/news?symbols={symbol}&limit={limit}";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AlpacaNewsResponse>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return result?.News ?? new List<AlpacaNewsItem>();
        }

        /// <summary>
        /// Retrieves the list of most active stocks from Alpaca screener.
        /// </summary>
        /// <returns>An <see cref="AlpacaMostActiveResponse"/> containing the most active stocks.</returns>
        public async Task <AlpacaMostActiveResponse> GetMostActivesAsync()
        {
            var url = $"/v1beta1/screener/stocks/most-actives";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var contentStream = await response.Content.ReadAsStreamAsync();

            var result = await JsonSerializer.DeserializeAsync<AlpacaMostActiveResponse>(contentStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result!;
        }

        /// <summary>
        /// Retrieves market movers for a given market type.
        /// </summary>
        /// <param name="marketType">Market type such as "nasdaq", "nyse", etc.</param>
        /// <returns>An <see cref="AlpacaMarketMoversResponse"/> containing movers data.</returns>
        public async Task<AlpacaMarketMoversResponse> GetMarketMoversAsync(string marketType)
        {
            var url = $"/v1beta1/screener/{marketType}/movers";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var contentStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<AlpacaMarketMoversResponse>(contentStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result!;

        }



        /// <summary>
        /// Searches the cached symbol metadata for symbols or names containing the given query string.
        /// </summary>
        /// <param name="query">Query string to search for.</param>
        /// <returns>Top 10 matching <see cref="SymbolLookupResult"/> objects.</returns>
        public Task<List<SymbolLookupResult>> SearchAsync(string query)
        {
            query = query.ToUpperInvariant();

            var results = _symbolCache
                .Where(s =>
                    s.Symbol.ToUpperInvariant().Contains(query) ||
                    s.Name.ToUpperInvariant().Contains(query))
                .OrderByDescending(s => s.Symbol.Equals(query, StringComparison.OrdinalIgnoreCase)) // exact matches
                .ThenBy(s => !s.Symbol.StartsWith(query, StringComparison.OrdinalIgnoreCase))        // starts with
                .ThenBy(s => s.Symbol)                                                               // alphabetical
                .Take(10)
                .ToList();
            return Task.FromResult(results);

        }

        /// <summary>
        /// Initializes the service by loading symbol metadata cache from Alpaca API.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InitializeAsync()
        {
            await LoadSymbolMetadataAsync();
        }


        /// <summary>
        /// Loads and caches active tradable asset metadata from Alpaca API.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task LoadSymbolMetadataAsync()
        {
            await _loadLock.WaitAsync();
            try
            {
                var url = "https://paper-api.alpaca.markets/v2/assets?status=active&tradable=true"; 

                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("Raw Alpaca assets response: ");
                Console.WriteLine(content);
                var assets = JsonSerializer.Deserialize<List<AlpacaAsset>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                Console.WriteLine($"Total assets fetched: {assets?.Count}");

                _symbolCache = assets!
                    .Select(a => new SymbolLookupResult
                    {
                        Symbol = a.Symbol,
                        Name = a.Name
                    })
                    .OrderBy(r => r.Symbol)
                    .ToList();
            }
            finally
            {
                _loadLock.Release();
            }
        }


        //public async Task<List<CorporateActionItem>> GetCorporateActionsAsync(string symbol, int limit = 10)
        //{
        //    var url = $"/v1/corporate-actions?symbols={symbol}&limit={limit}";
        //    var response = await _client.GetAsync(url);
        //    response.EnsureSuccessStatusCode();
        //    var json = await response.Content.ReadAsStringAsync();
        //    var result = JsonSerializer.Deserialize<AlpacaCorporateActionResponse>(json, new JsonSerializerOptions
        //    {
        //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //    });
        //    return result?.CorporateActions ?? new List<AlpacaCorporateActionItem>();

        //}












        //public AlpacaStockMarketService(IConfiguration configuration, ILogger<AlpacaStockMarketService> logger)
        //{
        //    _logger = logger;

        //    string apiKey = configuration["Alpaca:ApiKey"];
        //    string apiSecret = configuration["Alpaca:ApiSecret"];

        //    Console.WriteLine($"alpacsa key ={apiKey}");
        //    Console.WriteLine($"alpacsa secret ={apiSecret}");

        //    if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
        //    {
        //        throw new Exception("Alpaca API credentials are missing!");
        //    }
        //    //_logger.LogInformation($"API secret: {apiSecret}");
        //    //_logger.LogInformation($"API Key: {apiKey}");

        //    var securityKey = new SecretKey(apiKey, apiSecret);
        //    _dataClient = Alpaca.Markets.Environments.Paper.GetAlpacaDataStreamingClient(securityKey);
        //    _tradingClient = Alpaca.Markets.Environments.Paper.GetAlpacaTradingClient(securityKey); // ✅ Initialize Trading Client
        //}

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Connecting to Alpaca Data WebSocket...");
        //        await _dataClient.ConnectAndAuthenticateAsync(stoppingToken);
        //        _logger.LogInformation("Connected to Alpaca Data WebSocket.");

        //        // ✅ Check if the market is open
        //        var clock = await _tradingClient.GetClockAsync();

        //        _logger.LogInformation($"Market Open: {clock.IsOpen}");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error connecting to Alpaca Data WebSocket: {ex.Message}");
        //        _logger.LogError($"Stack Trace: {ex.StackTrace}");

        //    }
        //}

        //public async Task SubscribeToStockAsync(string symbol, CancellationToken stoppingToken)
        //{
        //    if (_subscribedSymbols.Contains(symbol))
        //    {
        //        _logger.LogInformation($"Already subscribed to {symbol}.");
        //        return;
        //    }

        //    _subscribedSymbols.Add(symbol);
        //    _logger.LogInformation($"Subscribing to {symbol}...");

        //    try
        //    {
        //        // Subscribe to trade updates
        //        _logger.LogInformation($"Setting up trade subscription for {symbol}...");
        //        var tradeSubscription = _dataClient.GetTradeSubscription(symbol);
        //        tradeSubscription.Received += trade =>
        //        {
        //            _logger.LogInformation($"Trade Update - Symbol: {trade.Symbol}, Price: {trade.Price}, Quantity: {trade.Size}, Time: {trade.TimestampUtc}");
        //        };
        //        await _dataClient.SubscribeAsync(tradeSubscription, stoppingToken);
        //        _logger.LogInformation($"Trade subscription for {symbol} completed. Subscribed: {tradeSubscription.Subscribed}");

        //        // Subscribe to quote updates
        //        _logger.LogInformation($"Setting up quote subscription for {symbol}...");
        //        var quoteSubscription = _dataClient.GetQuoteSubscription(symbol);
        //        quoteSubscription.Received += quote =>
        //        {
        //            _logger.LogInformation($"Quote Update - Symbol: {quote.Symbol}, Ask: {quote.AskPrice}, Bid: {quote.BidPrice}");
        //        };
        //        await _dataClient.SubscribeAsync(quoteSubscription, stoppingToken);
        //        _logger.LogInformation($"Quote subscription for {symbol} completed. Subscribed: {quoteSubscription.Subscribed}");

        //        // Subscribe to bar updates
        //        _logger.LogInformation($"Setting up bar subscription for {symbol}...");
        //        var barSubscription = _dataClient.GetMinuteBarSubscription(symbol);
        //        barSubscription.Received += bar =>
        //        {
        //            _logger.LogInformation($"Bar Update - Symbol: {bar.Symbol}, Open: {bar.Open}, High: {bar.High}, Low: {bar.Low}, Close: {bar.Close}, Volume: {bar.Volume}, Time: {bar.TimeUtc}");
        //        };
        //        await _dataClient.SubscribeAsync(barSubscription, stoppingToken);
        //        _logger.LogInformation($"Bar subscription for {symbol} completed. Subscribed: {barSubscription.Subscribed}");

        //        _logger.LogInformation($"Successfully subscribed to {symbol} for trade, quote, and bar updates.");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error subscribing to {symbol}: {ex.Message}");
        //        _logger.LogError($"Stack Trace: {ex.StackTrace}");
        //    }
        //}
    }
}
