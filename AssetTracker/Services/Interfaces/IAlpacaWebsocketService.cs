using AssetTracker.Models.MarketDataUpdates;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models.Enums;

namespace AssetTracker.Services
{
    /// <summary>
    /// Interface for managing a WebSocket connection to the Alpaca streaming service.
    /// </summary>
    public interface IAlpacaWebSocketService : IDisposable
    {
        /// <summary>
        /// Gets the most recent trade updates for subscribed symbols.
        /// </summary>
        IReadOnlyDictionary<string, TradeUpdate> LatestTrades { get; }

        /// <summary>
        /// Gets the most recent quote updates for subscribed symbols.
        /// </summary>
        IReadOnlyDictionary<string, QuoteUpdate> LatestQuotes { get; }

        /// <summary>
        /// Gets the most recent bar updates for subscribed symbols.
        /// </summary>
        IReadOnlyDictionary<string, BarUpdate> LatestBars { get; }

        /// <summary>
        /// Gets the current connection state of the WebSocket.
        /// </summary>
        ConnectionState State { get; }

        /// <summary>
        /// Starts the WebSocket connection.
        /// </summary>
        /// <returns>True if connection is successfully started; otherwise false.</returns>
        Task<bool> StartSocketAsync();

        /// <summary>
        /// Stops the WebSocket connection.
        /// </summary>
        /// <returns>True if successfully stopped; otherwise false.</returns>
        Task<bool> StopSocketAsync();

        /// <summary>
        /// Forcefully disconnects the WebSocket connection.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Notifies the service that a user has connected (used for managing user-specific streams).
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task NotifyUserConnectedAsync();

        /// <summary>
        /// Notifies the service that a user has disconnected.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task NotifyUserDisconnectedAsync();

        /// <summary>
        /// Subscribes to real-time trade updates for a given symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol to subscribe to.</param>
        Task SubscribeToTradesAsync(string symbol);

        /// <summary>
        /// Subscribes to real-time quote updates for a given symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol to subscribe to.</param>
        Task SubscribeToQuotesAsync(string symbol);

        /// <summary>
        /// Subscribes to real-time bar updates (e.g., 1-minute bars) for a given symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol to subscribe to.</param>
        Task SubscribeToBarsAsync(string symbol);

        /// <summary>
        /// Subscribes to trades, quotes, and bars for a given symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol to subscribe to all streams.</param>
        Task SubscribeToAllAsync(string symbol);

        /// <summary>
        /// Unsubscribes from trade updates for a given symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol to unsubscribe from trades.</param>
        Task UnsubscribeFromTradesAsync(string symbol);

        /// <summary>
        /// Unsubscribes from quote updates for a given symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol to unsubscribe from quotes.</param>
        Task UnsubscribeFromQuotesAsync(string symbol);

        /// <summary>
        /// Unsubscribes from bar updates for a given symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol to unsubscribe from bars.</param>
        Task UnsubscribeFromBarsAsync(string symbol);

        /// <summary>
        /// Unsubscribes from all updates (trades, quotes, bars) for a given symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol to unsubscribe from all streams.</param>
        Task UnsubscribeFromAllAsync(string symbol);
    }
}