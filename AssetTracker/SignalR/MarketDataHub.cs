using Microsoft.AspNetCore.SignalR;


/// <summary>
/// SignalR Hub for handling realtime market data subscriptions and group management.
/// </summary>
public class MarketDataHub : Hub
{
    private readonly AlpacaWebSocketService _webSocketService;
    private readonly ILogger<MarketDataHub> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarketDataHub"/> class.
    /// </summary>
    /// <param name="service">The Alpaca WebSocket service used for notifications.</param>
    /// <param name="logger">Logger for diagnostic information.</param>
    public MarketDataHub(AlpacaWebSocketService service, ILogger <MarketDataHub> logger)
    {
        _webSocketService = service;
        _logger = logger;
    }

    /// <summary>
    /// Subscribes the current connection to a specific symbol group.
    /// </summary>
    /// <param name="symbol">The stock symbol to subscribe to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SubscribeSymbol(string symbol)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, symbol);
    }

    /// <summary>
    /// Adds the current connection to a SignalR group for the given stock symbol.
    /// </summary>
    /// <param name="symbol">The symbol group to join.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task JoinGroup(string symbol)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol cannot be null or empty");

            await Groups.AddToGroupAsync(Context.ConnectionId, symbol);
            _logger.LogInformation($"Client {Context.ConnectionId} joined group {symbol}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in JoinGroup");
            throw;
        }
    }

    /// <summary>
    /// Removes the current connection from a SignalR group for the given stock symbol.
    /// </summary>
    /// <param name="symbol">The symbol group to leave.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task LeaveGroup(string symbol)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol cannot be null or empty");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);
            _logger.LogInformation($"Client {Context.ConnectionId} joined group {symbol}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LeaveGroup");
            throw;
        }

    }


    /// <summary>
    /// Called when a new client connects to the hub.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"Client connected: {Context.ConnectionId}");
        await _webSocketService.NotifyUserConnectedAsync();
        await base.OnConnectedAsync();
    }


    /// <summary>
    /// Called when a client disconnects from the hub.
    /// </summary>
    /// <param name="exception">Optional exception that occurred during disconnect.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogWarning($"Client disconnected: {Context.ConnectionId}, reason: {exception?.Message}");
        await _webSocketService.NotifyUserDisconnectedAsync();
        await base.OnDisconnectedAsync(exception);
    }
}

