using Microsoft.AspNetCore.SignalR;

public class MarketDataHub : Hub
{
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
}

