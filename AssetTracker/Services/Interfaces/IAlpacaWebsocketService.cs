using AssetTracker.Models.MarketDataUpdates;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models.Enums;
namespace AssetTracker.Services
{
    public interface IAlpacaWebSocketService : IDisposable
    {
        IReadOnlyDictionary<string, TradeUpdate> LatestTrades { get; }
        IReadOnlyDictionary<string, QuoteUpdate> LatestQuotes { get; }
        IReadOnlyDictionary<string, BarUpdate> LatestBars { get; }
        ConnectionState State { get; }

        Task<bool> StartSocketAsync();
        Task<bool> StopSocketAsync();
        Task DisconnectAsync();

        Task NotifyUserConnectedAsync();
        Task NotifyUserDisconnectedAsync();

        Task SubscribeToTradesAsync(string symbol);
        Task SubscribeToQuotesAsync(string symbol);
        Task SubscribeToBarsAsync(string symbol);
        Task SubscribeToAllAsync(string symbol);

        Task UnsubscribeFromTradesAsync(string symbol);
        Task UnsubscribeFromQuotesAsync(string symbol);
        Task UnsubscribeFromBarsAsync(string symbol);
        Task UnsubscribeFromAllAsync(string symbol);
    }
}
