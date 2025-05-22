using Microsoft.AspNetCore.SignalR;

public class MarketDataHub : Hub
{
    private readonly AlpacaWebSocketService _webSocketService;

    public MarketDataHub(AlpacaWebSocketService service)
    {
        _webSocketService = service;
    }
    public async Task SubscribeSymbol(string symbol)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, symbol);
    }
    public async Task JoinGroup(string symbol)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, symbol);
    }

    public async Task LeaveGroup(string symbol)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);
    }
    public override async Task OnConnectedAsync()
    {
        await _webSocketService.NotifyUserConnectedAsync();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _webSocketService.NotifyUserDisconnectedAsync();
        await base.OnDisconnectedAsync(exception);
    }
}

