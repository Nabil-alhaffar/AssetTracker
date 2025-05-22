using Microsoft.AspNetCore.SignalR;

public class MarketDataHub : Hub
{
    private readonly AlpacaWebSocketService _webSocketService;
    private readonly ILogger<MarketDataHub> _logger;

    public MarketDataHub(AlpacaWebSocketService service, ILogger <MarketDataHub> logger)
    {
        _webSocketService = service;
        _logger = logger;
    }
    public async Task SubscribeSymbol(string symbol)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, symbol);
    }
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
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"Client connected: {Context.ConnectionId}");
        await _webSocketService.NotifyUserConnectedAsync();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogWarning($"Client disconnected: {Context.ConnectionId}, reason: {exception?.Message}");
        await _webSocketService.NotifyUserDisconnectedAsync();
        await base.OnDisconnectedAsync(exception);
    }
}

