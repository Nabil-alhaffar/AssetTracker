using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetTracker.Helpers
{

    /// <summary>
    /// Manages user subscriptions to stock symbols, tracking which users are subscribed to which symbols.
    /// Provides thread-safe subscribe and unsubscribe operations and events for subscription changes.
    /// </summary>
    public class SymbolSubscriptionManager
    {
        private readonly ConcurrentDictionary<string, HashSet<Guid>> _symbolUserMap = new();
        private readonly SemaphoreSlim _subscriptionLock = new(1, 1);

        // Events for subscription changes

        /// <summary>
        /// Event fired when a symbol gets its first subscriber.
        /// </summary>
        public event Func<string, Task> OnSymbolSubscribed;

        /// <summary>
        /// Event fired when a symbol loses its last subscriber.
        /// </summary>
        public event Func<string, Task> OnSymbolUnsubscribed;


        /// <summary>
        /// Subscribes a user to a given symbol.
        /// If the symbol is new (no subscribers), triggers <see cref="OnSymbolSubscribed"/>.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="symbol">The stock symbol to subscribe to.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="symbol"/> is null or whitespace.</exception>
        public async Task SubscribeUserToSymbolAsync(Guid userId, string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol cannot be null or whitespace.", nameof(symbol));

            await _subscriptionLock.WaitAsync();
            try
            {
                if (!_symbolUserMap.ContainsKey(symbol))
                {
                    _symbolUserMap[symbol] = new HashSet<Guid>();
                    if (OnSymbolSubscribed != null)
                    {
                        await OnSymbolSubscribed.Invoke(symbol);
                    }
                }

                _symbolUserMap[symbol].Add(userId);
            }
            finally
            {
                _subscriptionLock.Release();
            }
        }


        /// <summary>
        /// Unsubscribes a user from a given symbol.
        /// If the symbol loses all subscribers, triggers <see cref="OnSymbolUnsubscribed"/>.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="symbol">The stock symbol to unsubscribe from.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="symbol"/> is null or whitespace.</exception>
        public async Task UnsubscribeUserFromSymbolAsync(Guid userId, string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol cannot be null or whitespace.", nameof(symbol));

            await _subscriptionLock.WaitAsync();
            try
            {
                if (_symbolUserMap.TryGetValue(symbol, out var users))
                {
                    users.Remove(userId);

                    if (users.Count == 0)
                    {
                        _symbolUserMap.Remove(symbol, out _);
                        if (OnSymbolUnsubscribed != null)
                        {
                            await OnSymbolUnsubscribed.Invoke(symbol);
                        }
                    }
                }
            }
            finally
            {
                _subscriptionLock.Release();
            }
        }



        /// <summary>
        /// Unsubscribes a user from all symbols they are subscribed to.
        /// Fires <see cref="OnSymbolUnsubscribed"/> for each symbol that loses its last subscriber.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        public async Task UnsubscribeUserFromAllAsync(Guid userId)
        {
            await _subscriptionLock.WaitAsync();
            try
            {
                var symbolsToUnsubscribe = new List<string>();

                foreach (var kvp in _symbolUserMap)
                {
                    kvp.Value.Remove(userId);
                    if (kvp.Value.Count == 0)
                    {
                        symbolsToUnsubscribe.Add(kvp.Key);
                    }
                }

                foreach (var symbol in symbolsToUnsubscribe)
                {
                    _symbolUserMap.Remove(symbol, out _);
                    if (OnSymbolUnsubscribed != null)
                    {
                        await OnSymbolUnsubscribed.Invoke(symbol);
                    }
                }
            }
            finally
            {
                _subscriptionLock.Release();
            }
        }


        /// <summary>
        /// Gets a read-only collection of symbols a user is currently subscribed to.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A read-only collection of subscribed symbols.</returns>
        public IReadOnlyCollection<string> GetUserSubscribedSymbols(Guid userId)
        {
            var symbols = new List<string>();

            foreach (var kvp in _symbolUserMap)
            {
                if (kvp.Value.Contains(userId))
                {
                    symbols.Add(kvp.Key);
                }
            }

            return symbols.AsReadOnly();
        }

        /// <summary>
        /// Gets a read-only collection of user IDs subscribed to a given symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol to query.</param>
        /// <returns>A read-only collection of user IDs subscribed to the symbol, or an empty collection if none.</returns>
        public IReadOnlyCollection<Guid> GetUsersSubscribedToSymbol(string symbol)
        {
            if (_symbolUserMap.TryGetValue(symbol, out var users))
            {
                return new List<Guid>(users).AsReadOnly();
            }

            return Array.Empty<Guid>();
        }

        /// <summary>
        /// Gets all symbols currently tracked with subscribers.
        /// </summary>
        /// <returns>An enumerable of all subscribed symbols.</returns>
        public IEnumerable<string> GetAllSubscribedSymbols()
        {
            return _symbolUserMap.Keys;
        }
    }
}
