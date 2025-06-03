using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetTracker.Helpers
{
    public class SymbolSubscriptionManager
    {
        private readonly ConcurrentDictionary<string, HashSet<Guid>> _symbolUserMap = new();
        private readonly SemaphoreSlim _subscriptionLock = new(1, 1);

        // Events for subscription changes
        public event Func<string, Task> OnSymbolSubscribed;
        public event Func<string, Task> OnSymbolUnsubscribed;

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

        public IEnumerable<string> GetAllSubscribedSymbols()
        {
            return _symbolUserMap.Keys;
        }
    }
}
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using AssetTracker.Services;
//namespace AssetTracker.Helpers
//{
//    public class SymbolSubscriptionManager
//    {
//        private readonly IAlpacaWebSocketService _alpacaWebSocketService;
//        private readonly ConcurrentDictionary<string, HashSet<Guid>> _symbolUserMap = new();
//        private readonly SemaphoreSlim _subscriptionLock = new(1, 1);

//        public SymbolSubscriptionManager(IAlpacaWebSocketService webSocketService)
//        {
//            _alpacaWebSocketService = webSocketService;
//        }

//        public async Task SubscribeUserToSymbolAsync(Guid userId, string symbol)
//        {
//            if (string.IsNullOrWhiteSpace(symbol))
//                throw new ArgumentException("Symbol cannot be null or whitespace.", nameof(symbol));

//            await _subscriptionLock.WaitAsync();
//            try
//            {
//                if (!_symbolUserMap.ContainsKey(symbol))
//                {
//                    _symbolUserMap[symbol] = new HashSet<Guid>();
//                    await _alpacaWebSocketService.SubscribeToAllAsync(symbol);
//                }

//                _symbolUserMap[symbol].Add(userId);
//            }
//            finally
//            {
//                _subscriptionLock.Release();
//            }
//        }

//        public async Task UnsubscribeUserFromSymbolAsync(Guid userId, string symbol)
//        {
//            if (string.IsNullOrWhiteSpace(symbol))
//                throw new ArgumentException("Symbol cannot be null or whitespace.", nameof(symbol));

//            await _subscriptionLock.WaitAsync();
//            try
//            {
//                if (_symbolUserMap.TryGetValue(symbol, out var users))
//                {
//                    users.Remove(userId);

//                    if (users.Count == 0)
//                    {
//                        _symbolUserMap.Remove(symbol, out _);
//                        await _alpacaWebSocketService.UnsubscribeFromAllAsync(symbol);
//                    }
//                }
//            }
//            finally
//            {
//                _subscriptionLock.Release();
//            }
//        }

//        public async Task UnsubscribeUserFromAllAsync(Guid userId)
//        {
//            await _subscriptionLock.WaitAsync();
//            try
//            {
//                var symbolsToUnsubscribe = new List<string>();

//                foreach (var kvp in _symbolUserMap)
//                {
//                    kvp.Value.Remove(userId);
//                    if (kvp.Value.Count == 0)
//                    {
//                        symbolsToUnsubscribe.Add(kvp.Key);
//                    }
//                }

//                foreach (var symbol in symbolsToUnsubscribe)
//                {
//                    _symbolUserMap.Remove(symbol, out _);
//                    await _alpacaWebSocketService.UnsubscribeFromAllAsync(symbol);
//                }
//            }
//            finally
//            {
//                _subscriptionLock.Release();
//            }
//        }

//        public IReadOnlyCollection<string> GetUserSubscribedSymbols(Guid userId)
//        {
//            var symbols = new List<string>();

//            foreach (var kvp in _symbolUserMap)
//            {
//                if (kvp.Value.Contains(userId))
//                {
//                    symbols.Add(kvp.Key);
//                }
//            }

//            return symbols.AsReadOnly();
//        }

//        public IReadOnlyCollection<Guid> GetUsersSubscribedToSymbol(string symbol)
//        {
//            if (_symbolUserMap.TryGetValue(symbol, out var users))
//            {
//                return new List<Guid>(users).AsReadOnly();
//            }

//            return Array.Empty<Guid>();
//        }

//        public IEnumerable<string> GetAllSubscribedSymbols()
//        {
//            return _symbolUserMap.Keys;
//        }
//    }

//}

