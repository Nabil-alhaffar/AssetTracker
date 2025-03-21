using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AssetTracker.Services.Interfaces;

namespace AssetTracker.Services
{
    public class AlpacaStockMarketService : BackgroundService, IAlpacaStockMarketService
    {
        private readonly ILogger<AlpacaStockMarketService> _logger;
        private readonly IAlpacaDataStreamingClient _dataClient;
        private readonly IAlpacaTradingClient _tradingClient; // ✅ Add Trading Client
        private readonly List<string> _subscribedSymbols = new();

        public AlpacaStockMarketService(IConfiguration configuration, ILogger<AlpacaStockMarketService> logger,
                                        IAlpacaDataStreamingClient dataClient, IAlpacaTradingClient tradingClient) // Injected dependencies
        {
            _logger = logger;
            _dataClient = dataClient;
            _tradingClient = tradingClient;
        }
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Connecting to Alpaca Data WebSocket...");
                await _dataClient.ConnectAndAuthenticateAsync(stoppingToken);
                _logger.LogInformation("Connected to Alpaca Data WebSocket.");

                // ✅ Check if the market is open
                var clock = await _tradingClient.GetClockAsync();
                
                _logger.LogInformation($"Market Open: {clock.IsOpen}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error connecting to Alpaca Data WebSocket: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");

            }
        }

        public async Task SubscribeToStockAsync(string symbol, CancellationToken stoppingToken)
        {
            if (_subscribedSymbols.Contains(symbol))
            {
                _logger.LogInformation($"Already subscribed to {symbol}.");
                return;
            }

            _subscribedSymbols.Add(symbol);
            _logger.LogInformation($"Subscribing to {symbol}...");

            try
            {
                // Subscribe to trade updates
                _logger.LogInformation($"Setting up trade subscription for {symbol}...");
                var tradeSubscription = _dataClient.GetTradeSubscription(symbol);
                tradeSubscription.Received += trade =>
                {
                    _logger.LogInformation($"Trade Update - Symbol: {trade.Symbol}, Price: {trade.Price}, Quantity: {trade.Size}, Time: {trade.TimestampUtc}");
                };
                await _dataClient.SubscribeAsync(tradeSubscription, stoppingToken);
                _logger.LogInformation($"Trade subscription for {symbol} completed. Subscribed: {tradeSubscription.Subscribed}");

                // Subscribe to quote updates
                _logger.LogInformation($"Setting up quote subscription for {symbol}...");
                var quoteSubscription = _dataClient.GetQuoteSubscription(symbol);
                quoteSubscription.Received += quote =>
                {
                    _logger.LogInformation($"Quote Update - Symbol: {quote.Symbol}, Ask: {quote.AskPrice}, Bid: {quote.BidPrice}");
                };
                await _dataClient.SubscribeAsync(quoteSubscription, stoppingToken);
                _logger.LogInformation($"Quote subscription for {symbol} completed. Subscribed: {quoteSubscription.Subscribed}");

                // Subscribe to bar updates
                _logger.LogInformation($"Setting up bar subscription for {symbol}...");
                var barSubscription = _dataClient.GetMinuteBarSubscription(symbol);
                barSubscription.Received += bar =>
                {
                    _logger.LogInformation($"Bar Update - Symbol: {bar.Symbol}, Open: {bar.Open}, High: {bar.High}, Low: {bar.Low}, Close: {bar.Close}, Volume: {bar.Volume}, Time: {bar.TimeUtc}");
                };
                await _dataClient.SubscribeAsync(barSubscription, stoppingToken);
                _logger.LogInformation($"Bar subscription for {symbol} completed. Subscribed: {barSubscription.Subscribed}");

                _logger.LogInformation($"Successfully subscribed to {symbol} for trade, quote, and bar updates.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error subscribing to {symbol}: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}
