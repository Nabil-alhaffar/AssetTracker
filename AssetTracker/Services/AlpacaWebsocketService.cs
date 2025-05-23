//// AlpacaWebSocketService.cs
///


using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;
using AssetTracker.Models.MarketDataUpdates;

public class AlpacaWebSocketService : BackgroundService, IDisposable
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

    private int _activeUserCount = 0;
    private readonly object _userLock = new();

    private CancellationTokenSource _socketCts = new();
    private Task _socketTask;
    public enum ConnectionState { Stopped, Starting, Running, Stopping }

    public volatile ConnectionState _state = ConnectionState.Stopped;
    private readonly object _stateLock = new();

    public IReadOnlyDictionary<string, TradeUpdate> LatestTrades => _latestTrades;
    public IReadOnlyDictionary<string, QuoteUpdate> LatestQuotes => _latestQuotes;
    public IReadOnlyDictionary<string, BarUpdate> LatestBars => _latestBars;

    public AlpacaWebSocketService(IServiceProvider serviceProvider, IConfiguration config)
    {
        _apiKey = config["Alpaca:ApiKey"];
        _apiSecret = config["Alpaca:ApiSecret"];
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask; // Real socket run is controlled by StartSocketAsync
    }

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
    public async Task NotifyUserConnectedAsync()
    {
        lock (_userLock)
        {
            _activeUserCount++;
        }

        if (_state == ConnectionState.Stopped)
            await StartSocketAsync();
    }
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
                await ResubscribeAllAsync();

                lock (_stateLock) { _state = ConnectionState.Running; }

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

    private async Task ResubscribeAllAsync()
    {
        var currentSubscriptions = _subscribedSymbols.Keys.ToList();
        _subscribedSymbols.Clear();

        foreach (var key in currentSubscriptions)
        {
            var parts = key.Split('_');
            if (parts.Length != 2) continue;

            var type = parts[0];
            var symbol = parts[1];

            await SubscribeAsync(symbol, isQuote: type == "Q", isBar: type == "B");
        }
    }

    public async Task SubscribeAsync(string symbol, bool isQuote = false, bool isBar = false)
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

    public Task SubscribeToAllAsync(string symbol) =>
        Task.WhenAll(
            SubscribeAsync(symbol),
            SubscribeAsync(symbol, isQuote: true),
            SubscribeAsync(symbol, isBar: true)
        );

    public Task SubscribeToTradesAsync(string symbol) => SubscribeAsync(symbol);
    public Task SubscribeToQuotesAsync(string symbol) => SubscribeAsync(symbol, isQuote: true);
    public Task SubscribeToBarsAsync(string symbol) => SubscribeAsync(symbol, isBar: true);

    public async Task UnsubscribeAsync(string symbol, bool isQuote = false, bool isBar = false)
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

    public Task UnsubscribeFromTradesAsync(string symbol) => UnsubscribeAsync(symbol);
    public Task UnsubscribeFromQuotesAsync(string symbol) => UnsubscribeAsync(symbol, isQuote: true);
    public Task UnsubscribeFromBarsAsync(string symbol) => UnsubscribeAsync(symbol, isBar: true);
    public async Task UnsubscribeFromAllAsync(string symbol)
    {
        await Task.WhenAll(
            UnsubscribeFromTradesAsync(symbol),
            UnsubscribeFromQuotesAsync(symbol),
            UnsubscribeFromBarsAsync(symbol)
        );
    }

    public async Task DisconnectAsync() => await StopSocketAsync();

    public void Dispose()
    {
        _socket?.Dispose();
        _sendLock.Dispose();
        _socketCts?.Dispose();
    }



}

//using Microsoft.Extensions.Hosting;
//using Microsoft.AspNetCore.SignalR;
//using System.Net.WebSockets;
//using System.Text;
//using System.Text.Json;
//using System.Collections.Concurrent;
//using AssetTracker.Models.MarketDataUpdates;
//using System.Net.Sockets;

//public class AlpacaWebSocketService : BackgroundService  , IDisposable
//{
//    private ClientWebSocket _socket = new();
//    private const string Url = "wss://stream.data.alpaca.markets/v2/iex";
//    private readonly string _apiKey;
//    private readonly string _apiSecret;
//    private readonly ConcurrentDictionary<string, byte> _subscribedSymbols = new();
//    private readonly SemaphoreSlim _sendLock = new(1, 1);
//    private readonly ConcurrentDictionary<string, TradeUpdate> _latestTrades = new();
//    private readonly ConcurrentDictionary<string, QuoteUpdate> _latestQuotes = new();
//    private readonly ConcurrentDictionary<string, BarUpdate> _latestBars = new();
//    private readonly IServiceProvider _serviceProvider;
//    private bool _isStopped = false;

//    public IReadOnlyDictionary<string, TradeUpdate> LatestTrades => _latestTrades;
//    public IReadOnlyDictionary<string, QuoteUpdate> LatestQuotes => _latestQuotes;
//    public IReadOnlyDictionary<string, BarUpdate> LatestBars => _latestBars;

//    public AlpacaWebSocketService(IServiceProvider serviceProvider, IConfiguration config)
//    {
//        _apiKey = config["Alpaca:ApiKey"];
//        _apiSecret = config["Alpaca:ApiSecret"];
//        _serviceProvider = serviceProvider;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        var buffer = new byte[8192];

//        while (!_isStopped &&!stoppingToken.IsCancellationRequested)
//        {
//            try
//            {
//                if (_socket != null)
//                {
//                    _socket.Dispose();
//                }
//                _socket = new();
//                await _socket.ConnectAsync(new Uri(Url), stoppingToken);

//                var authMsg = JsonSerializer.Serialize(new { action = "auth", key = _apiKey, secret = _apiSecret });
//                await SendMessageAsync(authMsg);
//                await ResubscribeAllAsync();

//                //await SubscribeAsync("AAPL");
//                //await SubscribeAsync("AMD", isQuote: true);
//                //await SubscribeAsync("MSFT", isBar: true);

//                while (!_isStopped && !stoppingToken.IsCancellationRequested && _socket.State == WebSocketState.Open)
//                {
//                    var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
//                    if (result.MessageType == WebSocketMessageType.Close)
//                        break;

//                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
//                    Console.WriteLine("Received: " + message);

//                    try
//                    {
//                        var options = new JsonSerializerOptions
//                        {
//                            PropertyNameCaseInsensitive = false
//                        };
//                        var messages = JsonSerializer.Deserialize<List<JsonElement>>(message,options);

//                        foreach (var msg in messages)
//                        {

//                            try
//                            {
//                                var type = msg.GetProperty("T").GetString();
//                                using var scope = _serviceProvider.CreateScope();
//                                var hub = scope.ServiceProvider.GetRequiredService<IHubContext<MarketDataHub>>();

//                                switch (type)
//                                {
//                                    case "t":
//                                        var trade = JsonSerializer.Deserialize<TradeUpdate>(msg.GetRawText(),options);
//                                        _latestTrades[trade.Symbol] = trade;
//                                        await hub.Clients.Group(trade.Symbol).SendAsync("ReceiveTrade", trade);
//                                        break;
//                                    case "q":
//                                        var quote = JsonSerializer.Deserialize<QuoteUpdate>(msg.GetRawText(),options);
//                                        _latestQuotes[quote.Symbol] = quote;
//                                        await hub.Clients.Group(quote.Symbol).SendAsync("ReceiveQuote", quote);
//                                        break;
//                                    case "b":
//                                        var bar = JsonSerializer.Deserialize<BarUpdate>(msg.GetRawText(), options);
//                                        _latestBars[bar.Symbol] = bar;
//                                        await hub.Clients.Group(bar.Symbol).SendAsync("ReceiveBar", bar);
//                                        break;
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                Console.WriteLine($"Error handling message: {msg}, Exception: {ex}");
//                            }
//                            if (_isStopped || stoppingToken.IsCancellationRequested)
//                                break;

//                            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
//                        }
//                    }
//                    catch (JsonException ex)
//                    {
//                        Console.WriteLine($"JSON parse error: {ex.Message} - Raw message: {message}");
//                    }
//                }
//            }
//            catch (WebSocketException ex)
//            {
//                Console.WriteLine($"WebSocket closed unexpectedly: {ex.Message}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Unhandled exception in AlpacaWebSocketService: {ex}");
//            }

//            // Wait before attempting to reconnect
//            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
//        }
//    }


//    public async Task SubscribeAsync(string symbol, bool isQuote = false, bool isBar = false)
//    {
//        if (string.IsNullOrWhiteSpace(symbol)) return;
//        var key = $"{(isQuote ? "Q" : isBar ? "B" : "T")}_{symbol.ToUpper()}";

//        if (!_subscribedSymbols.TryAdd(key, 0)) return;

//        var subscribeMsg = JsonSerializer.Serialize(new
//        {
//            action = "subscribe",
//            trades = isQuote || isBar ? null : new[] { symbol },
//            quotes = isQuote ? new[] { symbol } : null,
//            bars = isBar ? new[] { symbol } : null
//        });

//        await SendMessageAsync(subscribeMsg);
//    }

//    public async Task SubscribeToAllAsync(string symbol)
//    {
//        await SubscribeAsync(symbol);                      // Trades
//        await SubscribeAsync(symbol, isQuote: true);       // Quotes
//        await SubscribeAsync(symbol, isBar: true);         // Bars
//    }
//    public Task SubscribeToTradesAsync(string symbol) => SubscribeAsync(symbol);

//    public Task SubscribeToQuotesAsync(string symbol) => SubscribeAsync(symbol, isQuote: true);

//    public Task SubscribeToBarsAsync(string symbol) => SubscribeAsync(symbol, isBar: true);




//    private async Task SendMessageAsync(string message)
//    {
//        if (_socket == null || _socket.State != WebSocketState.Open)
//            return;
//        var bytes = Encoding.UTF8.GetBytes(message);
//        await _sendLock.WaitAsync();
//        try
//        {
//            await _socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
//        }
//        finally
//        {
//            _sendLock.Release();
//        }
//    }
//    private async Task ResubscribeAllAsync()
//    {
//        var currentSubscriptions = _subscribedSymbols.Keys.ToList();
//        _subscribedSymbols.Clear(); // Clear so SubscribeAsync will re-send

//        foreach (var key in currentSubscriptions)
//        {
//            var parts = key.Split('_');
//            if (parts.Length != 2) continue;

//            var type = parts[0];
//            var symbol = parts[1];

//            await SubscribeAsync(symbol, isQuote: type == "Q", isBar: type == "B");
//        }
//    }


//    public async Task UnsubscribeAsync(string symbol, bool isQuote = false, bool isBar = false)
//    {
//        if (string.IsNullOrWhiteSpace(symbol)) return;

//        var key = $"{(isQuote ? "Q" : isBar ? "B" : "T")}_{symbol.ToUpper()}";
//        if (!_subscribedSymbols.TryRemove(key, out _)) return;

//        var unsubscribeMsg = JsonSerializer.Serialize(new
//        {
//            action = "unsubscribe",
//            trades = !isQuote && !isBar ? new[] { symbol } : null,
//            quotes = isQuote ? new[] { symbol } : null,
//            bars = isBar ? new[] { symbol } : null
//        });

//        await SendMessageAsync(unsubscribeMsg);
//    }
//    public Task UnsubscribeFromTradesAsync(string symbol) => UnsubscribeAsync(symbol);

//    public Task UnsubscribeFromQuotesAsync(string symbol) => UnsubscribeAsync(symbol, isQuote: true);

//    public Task UnsubscribeFromBarsAsync(string symbol) => UnsubscribeAsync(symbol, isBar: true);

//    public async Task UnsubscribeFromAllAsync(string symbol)
//    {
//        await UnsubscribeFromTradesAsync(symbol);
//        await UnsubscribeFromQuotesAsync(symbol);
//        await UnsubscribeFromBarsAsync(symbol);
//    }

//    public async Task DisconnectAsync()
//    {
//        _isStopped = true;
//        if (_socket != null && _socket.State == WebSocketState.Open)
//        {
//            try
//            {
//                await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Client disconnect", CancellationToken.None);
//                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnect", CancellationToken.None);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error closing WebSocket: {ex.Message}");
//            }
//            finally
//            {
//                _socket.Dispose();
//                _socket = null;
//            }
//        }
//    }


//}

