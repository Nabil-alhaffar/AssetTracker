using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AssetTracker.Helpers
{
    public class SymbolSubscriptionManager
    {
        private readonly AlpacaWebSocketService _webSocketService;
        private readonly ConcurrentDictionary<string, HashSet<Guid>> _symbolUserMap = new();
        private readonly SemaphoreSlim _subscriptionLock = new(1, 1);

        public SymbolSubscriptionManager(AlpacaWebSocketService webSocketService)
        {
            _webSocketService = webSocketService;
        }

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
                    await _webSocketService.SubscribeAsync(symbol);
                }

                _symbolUserMap[symbol].Add(userId);
            }
            finally
            {
                _subscriptionLock.Release();
            }
        }

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
                        await _webSocketService.UnsubscribeAsync(symbol);
                    }
                }
            }
            finally
            {
                _subscriptionLock.Release();
            }
        }

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
                    await _webSocketService.UnsubscribeAsync(symbol);
                }
            }
            finally
            {
                _subscriptionLock.Release();
            }
        }

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

        public IReadOnlyCollection<Guid> GetUsersSubscribedToSymbol(string symbol)
        {
            if (_symbolUserMap.TryGetValue(symbol, out var users))
            {
                return new List<Guid>(users).AsReadOnly();
            }

            return Array.Empty<Guid>();
        }
    }

}

