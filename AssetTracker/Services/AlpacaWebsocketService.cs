//// AlpacaWebSocketService.cs

using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;
using AssetTracker.Models.MarketDataUpdates;
using AssetTracker.Helpers;
using AssetTracker.Services.Interfaces;
using AssetTracker.Services;
using AssetTracker.Models.Enums;

/// <summary>
/// Service to manage WebSocket connection to Alpaca market data streaming API.
/// Handles connection lifecycle, authentication, subscriptions to trade, quote, and bar updates,
/// and broadcasting updates via SignalR hubs.
/// </summary>
public class AlpacaWebSocketService : BackgroundService, IDisposable, IAlpacaWebSocketService
{
    private ClientWebSocket _socket;
    private const string Url = "wss://stream.data.alpaca.markets/v2/iex";
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly ConcurrentDictionary<string, byte> _subscribedSymbols = new();
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private readonly ConcurrentDictionary<string, TradeUpdate> _latestTrades = new();
    private readonly ConcurrentDictionary<string, QuoteUpdate> _latestQuotes = new();
    private readonly ConcurrentDictionary<string, BarUpdate> _latestBars = new();
    private readonly IServiceProvider _serviceProvider;
    private volatile ConnectionState _state = ConnectionState.Stopped;


    /// <summary>
    /// Gets the current connection state of the WebSocket.
    /// </summary>
    public ConnectionState State => _state;

    private int _activeUserCount = 0;
    private readonly object _userLock = new();

    private CancellationTokenSource _socketCts = new();
    private Task _socketTask;

    private readonly object _stateLock = new();

    /// <summary>
    /// Gets a read-only dictionary of the latest trade updates by symbol.
    /// </summary>
    public IReadOnlyDictionary<string, TradeUpdate> LatestTrades => _latestTrades;

    /// <summary>
    /// Gets a read-only dictionary of the latest quote updates by symbol.
    /// </summary>
    public IReadOnlyDictionary<string, QuoteUpdate> LatestQuotes => _latestQuotes;

    /// <summary>
    /// Gets a read-only dictionary of the latest bar updates by symbol.
    /// </summary>
    public IReadOnlyDictionary<string, BarUpdate> LatestBars => _latestBars;

    private readonly SymbolSubscriptionManager _symbolSubscriptionManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlpacaWebSocketService"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider to create scopes for DI services.</param>
    /// <param name="config">Configuration instance to retrieve Alpaca API credentials.</param>
    /// <param name="symbolSubscriptionManager">Manager to track symbol subscriptions.</param>
    public AlpacaWebSocketService(IServiceProvider serviceProvider, IConfiguration config, SymbolSubscriptionManager symbolSubscriptionManager)
    {
        _apiKey = config["Alpaca:ApiKey"];
        _apiSecret = config["Alpaca:ApiSecret"];
        _serviceProvider = serviceProvider;
        _symbolSubscriptionManager = symbolSubscriptionManager;

        // Subscribe to the events
        _symbolSubscriptionManager.OnSymbolSubscribed += SubscribeToAllAsync;
        _symbolSubscriptionManager.OnSymbolUnsubscribed += UnsubscribeFromAllAsync;
    }

    /// <summary>
    /// Executes the background service.
    /// This method is overridden but does not start the socket by default.
    /// Use <see cref="StartSocketAsync"/> to begin connection.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token to stop the service.</param>
    /// <returns>A completed task.</returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask; // Real socket run is controlled by StartSocketAsync
    }

    

    /// <summary>
    /// Starts the WebSocket connection asynchronously if not already running.
    /// </summary>
    /// <returns>True if the socket start was initiated; otherwise false if already running.</returns>
    public async Task <bool> StartSocketAsync()
    {
        lock (_stateLock)
        {
            if (_state is ConnectionState.Starting or ConnectionState.Running)
                return false;

            _state = ConnectionState.Starting;
            _socketCts = new CancellationTokenSource();
        }

        _socketTask =  Task.Run(() => RunSocketAsync(_socketCts.Token));
        return true;
    }

    /// <summary>
    /// Notifies the service that a new user connected and starts socket if needed.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task NotifyUserConnectedAsync()
    {
        lock (_userLock)
        {
            _activeUserCount++;
        }

        if (_state == ConnectionState.Stopped)
            await StartSocketAsync();
    }


    /// <summary>
    /// Notifies the service that a user disconnected.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task NotifyUserDisconnectedAsync()
    {
        //bool shouldStop = false;
        lock (_userLock)
        {
            _activeUserCount = Math.Max(0, _activeUserCount - 1);
            //shouldStop = _activeUserCount == 0;
        }

        //if (shouldStop)
        //    await StopSocketAsync();
    }

    /// <summary>
    /// Stops the WebSocket connection asynchronously if running.
    /// </summary>
    /// <returns>True if the socket stop was initiated; otherwise false if already stopped or stopping.</returns>
    public async Task <bool> StopSocketAsync()
    {
        lock (_stateLock)
        {
            if (_state is ConnectionState.Stopping or ConnectionState.Stopped)
            {

                _state = ConnectionState.Stopping;
                return false;
            }
                
        }

        _socketCts.Cancel();

        if (_socket != null && _socket.State == WebSocketState.Open)
        {
            try
            {
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnect", CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing WebSocket: {ex.Message}");
            }
        }

        _socket?.Dispose();
        _socket = null;

        lock (_stateLock)
        {
            _state = ConnectionState.Stopped;
        }
        return true;
    }

    /// <summary>
    /// Establishes and maintains a WebSocket connection to Alpaca's real-time market data stream.
    /// Handles authentication, listens for incoming messages, processes market updates,
    /// and broadcasts updates via SignalR to subscribed clients.
    /// Automatically attempts reconnection upon disconnection or error.
    /// </summary>
    /// <param name="stoppingToken">A cancellation token that signals when the socket should stop running.</param>
    /// <returns>A task that represents the asynchronous socket operation.</returns>
    private async Task RunSocketAsync(CancellationToken stoppingToken)
    {
        var buffer = new byte[8192];

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _socket = new ClientWebSocket();
                await _socket.ConnectAsync(new Uri(Url), stoppingToken);

                var authMsg = JsonSerializer.Serialize(new { action = "auth", key = _apiKey, secret = _apiSecret });
                await SendMessageAsync(authMsg);

                lock (_stateLock) { _state = ConnectionState.Running; }

                await ResubscribeAllAsync();

                while (_socket.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
                {
                    var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine("Received: " + message);

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
                    var messages = JsonSerializer.Deserialize<List<JsonElement>>(message, options);

                    foreach (var msg in messages)
                    {
                        var type = msg.GetProperty("T").GetString();
                        using var scope = _serviceProvider.CreateScope();
                        var hub = scope.ServiceProvider.GetRequiredService<IHubContext<MarketDataHub>>();

                        switch (type)
                        {
                            case "t":
                                var trade = JsonSerializer.Deserialize<TradeUpdate>(msg.GetRawText(), options);
                                _latestTrades[trade.Symbol] = trade;
                                try
                                {
                                    await hub.Clients.Group(trade.Symbol).SendAsync("ReceiveTrade", trade);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"SignalR trade send error: {ex}");
                                }
                                break;
                            case "q":
                                var quote = JsonSerializer.Deserialize<QuoteUpdate>(msg.GetRawText(), options);

                                _latestQuotes[quote.Symbol] = quote;

                                try
                                {
                                    await hub.Clients.Group(quote.Symbol).SendAsync("ReceiveQuote", quote);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"SignalR quote send error: {ex}");
                                }
                                break;
                            case "b":
                                var bar = JsonSerializer.Deserialize<BarUpdate>(msg.GetRawText(), options);
                                _latestBars[bar.Symbol] = bar;

                                try
                                {
                                    await hub.Clients.Group(bar.Symbol).SendAsync("ReceiveBar", bar);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"SignalR bar send error: {ex}");
                                }
                                break;
                        }

                        await Task.Delay(5, stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException) { break; }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket closed unexpectedly: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex}");
            }

            if (!stoppingToken.IsCancellationRequested)
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        lock (_stateLock) { _state = ConnectionState.Stopped; }
    }

    /// <summary>
    /// Sends a JSON-formatted message through the WebSocket connection.
    /// </summary>
    /// <param name="message">The JSON message to send.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    private async Task SendMessageAsync(string message)
    {
        if (_socket == null || _socket.State != WebSocketState.Open)
            return;

        var bytes = Encoding.UTF8.GetBytes(message);
        await _sendLock.WaitAsync();
        try
        {
            await _socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        finally
        {
            _sendLock.Release();
        }
    }
    /// <summary>
    /// Resubscribes to all currently subscribed symbols after a reconnect.
    /// </summary>
    /// <returns>A task representing the asynchronous resubscription operation.</returns>
    private async Task ResubscribeAllAsync()
    {
        // Wait until the socket is connected before resubscribing
        //while (_state != ConnectionState.Running)
        //{
        //    await Task.Delay(1000);
        //}

        var currentSubscriptions = _symbolSubscriptionManager.GetAllSubscribedSymbols().ToList();
        _subscribedSymbols.Clear();

        foreach (var symbol in currentSubscriptions)
        {
            //var parts = key.Split('_');
            //if (parts.Length != 2) continue;

            //var type = parts[0];
            //var symbol = parts[1];
            await SubscribeToAllAsync(symbol);
        

            //switch (type)
            //{
            //    case "T":
            //        await SubscribeToTradesAsync(symbol);
            //        break;
            //    case "Q":
            //        await SubscribeToQuotesAsync(symbol);
            //        break;
            //    case "B":
            //        await SubscribeToBarsAsync(symbol);
            //        break;
            //}
        }
    }


    //private async Task ResubscribeAllAsync()
    //{
    //    var currentSubscriptions = _subscribedSymbols.Keys.ToList();
    //    _subscribedSymbols.Clear();

    //    foreach (var key in currentSubscriptions)
    //    {
    //        var parts = key.Split('_');
    //        if (parts.Length != 2) continue;

    //        var type = parts[0];
    //        var symbol = parts[1];

    //        await SubscribeAsync(symbol, isQuote: type == "Q", isBar: type == "B");
    //    }
    //}

    /// <summary>
    /// Subscribes to trade, quote, or bar updates for a specific symbol.
    /// </summary>
    /// <param name="symbol">The symbol to subscribe to.</param>
    /// <param name="isQuote">True to subscribe to quote updates.</param>
    /// <param name="isBar">True to subscribe to bar updates.</param>
    /// <returns>A task representing the asynchronous subscription operation.</returns>
    private async Task SubscribeAsync(string symbol, bool isQuote = false, bool isBar = false)
    {
        if (string.IsNullOrWhiteSpace(symbol)) return;
        var key = $"{(isQuote ? "Q" : isBar ? "B" : "T")}_{symbol.ToUpper()}";
        if (!_subscribedSymbols.TryAdd(key, 0)) return;

        var subscribeMsg = JsonSerializer.Serialize(new
        {
            action = "subscribe",
            trades = isQuote || isBar ? null : new[] { symbol },
            quotes = isQuote ? new[] { symbol } : null,
            bars = isBar ? new[] { symbol } : null
        });

        await SendMessageAsync(subscribeMsg);
    }


    /// <summary>
    /// Subscribes to trades, quotes, and bars for a given symbol.
    /// </summary>
    /// <param name="symbol">The symbol to subscribe to all update types for.</param>
    /// <returns>A task that completes when all subscriptions are processed.</returns>
    public Task SubscribeToAllAsync(string symbol) =>
        Task.WhenAll(
            SubscribeAsync(symbol),
            SubscribeAsync(symbol, isQuote: true),
            SubscribeAsync(symbol, isBar: true)
        );


    /// <summary>
    /// Subscribes only to trade updates for a specific symbol.
    /// </summary>
    /// <param name="symbol">The symbol to subscribe to.</param>
    /// <returns>A task representing the asynchronous subscription operation.</returns>
    public Task SubscribeToTradesAsync(string symbol) => SubscribeAsync(symbol);


    /// <summary>
    /// Subscribes only to quote updates for a specific symbol.
    /// </summary>
    /// <param name="symbol">The symbol to subscribe to.</param>
    /// <returns>A task representing the asynchronous subscription operation.</returns>
    public Task SubscribeToQuotesAsync(string symbol) => SubscribeAsync(symbol, isQuote: true);


    /// <summary>
    /// Subscribes only to bar updates for a specific symbol.
    /// </summary>
    /// <param name="symbol">The symbol to subscribe to.</param>
    /// <returns>A task representing the asynchronous subscription operation.</returns>
    public Task SubscribeToBarsAsync(string symbol) => SubscribeAsync(symbol, isBar: true);

    /// <summary>
    /// Unsubscribes from trade, quote, or bar updates for a specific symbol.
    /// </summary>
    /// <param name="symbol">The symbol to unsubscribe from.</param>
    /// <param name="isQuote">True to unsubscribe from quote updates.</param>
    /// <param name="isBar">True to unsubscribe from bar updates.</param>
    /// <returns>A task representing the asynchronous unsubscribe operation.</returns>
    private async Task UnsubscribeAsync(string symbol, bool isQuote = false, bool isBar = false)
    {
        if (string.IsNullOrWhiteSpace(symbol)) return;

        var key = $"{(isQuote ? "Q" : isBar ? "B" : "T")}_{symbol.ToUpper()}";
        if (!_subscribedSymbols.TryRemove(key, out _)) return;

        var unsubscribeMsg = JsonSerializer.Serialize(new
        {
            action = "unsubscribe",
            trades = !isQuote && !isBar ? new[] { symbol } : null,
            quotes = isQuote ? new[] { symbol } : null,
            bars = isBar ? new[] { symbol } : null
        });

        await SendMessageAsync(unsubscribeMsg);
    }

    /// <summary>
    /// Unsubscribes from trade updates for a specific symbol.
    /// </summary>
    /// <param name="symbol">The symbol to unsubscribe from.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UnsubscribeFromTradesAsync(string symbol) => UnsubscribeAsync(symbol);


    /// <summary>
    /// Unsubscribes from quote updates for a specific symbol.
    /// </summary>
    /// <param name="symbol">The symbol to unsubscribe from.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UnsubscribeFromQuotesAsync(string symbol) => UnsubscribeAsync(symbol, isQuote: true);


    /// <summary>
    /// Unsubscribes from bar updates for a specific symbol.
    /// </summary>
    /// <param name="symbol">The symbol to unsubscribe from.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UnsubscribeFromBarsAsync(string symbol) => UnsubscribeAsync(symbol, isBar: true);


    /// <summary>
    /// Unsubscribes from all trade, quote, and bar updates for a specific symbol.
    /// </summary>
    /// <param name="symbol">The symbol to unsubscribe from all update types.</param>
    /// <returns>A task representing the asynchronous unsubscribe operation.</returns>
    public async Task UnsubscribeFromAllAsync(string symbol)
    {
        await Task.WhenAll(
            UnsubscribeFromTradesAsync(symbol),
            UnsubscribeFromQuotesAsync(symbol),
            UnsubscribeFromBarsAsync(symbol)
        );
    }

    /// <summary>
    /// Gracefully disconnects the WebSocket connection by stopping the socket.
    /// </summary>
    /// <returns>A task that represents the asynchronous disconnection process.</returns>
    public async Task DisconnectAsync() => await StopSocketAsync();


    /// <summary>
    /// Releases all managed resources and unsubscribes from symbol events overriding the method from IDisposable.
    /// </summary>
    public override void Dispose()
    {
        
        
            _socket?.Dispose();
            _sendLock.Dispose();
            _socketCts?.Dispose();

            _symbolSubscriptionManager.OnSymbolSubscribed -= SubscribeToAllAsync;
            _symbolSubscriptionManager.OnSymbolUnsubscribed -= UnsubscribeFromAllAsync;
        

        base.Dispose();
    }



}
