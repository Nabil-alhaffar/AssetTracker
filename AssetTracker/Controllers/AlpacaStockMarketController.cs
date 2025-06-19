using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using AssetTracker.Services;
using AssetTracker.Services.Interfaces;
using Alpaca.Markets;

namespace AssetTracker.Controllers
{
    [ApiController]
    [Route("api/alpaca")]
    public class AlpacaStockMarketController : ControllerBase
    {
        //private readonly IAlpacaStockMarketService _alpacaService;
        private readonly IAlpacaWebSocketService _alpacaWebSocketService;
        private readonly IAlpacaStockMarketService _alpacaStockMarketService;
        private readonly ILogger<AlpacaStockMarketController> _logger;

        // Constructor to inject WebSocket, stock market service, and logger
        public AlpacaStockMarketController(IAlpacaWebSocketService alpacaService, IAlpacaStockMarketService alpacaStockMarketService, ILogger<AlpacaStockMarketController> logger)
        {
            _alpacaWebSocketService = alpacaService;
            _alpacaStockMarketService = alpacaStockMarketService;
            _logger = logger;
        }


        /// <summary>
        /// Starts the Alpaca WebSocket connection.
        /// </summary>
        [HttpPost("start")]
        public async Task<IActionResult> Start()
        {
            var result = await _alpacaWebSocketService.StartSocketAsync();
            return Ok(new { message = result ? "Socket started." : "Socket was already running." });
        }

        /// <summary>
        /// Stops the Alpaca WebSocket connection.
        /// </summary>
        [HttpPost("stop")]
        public async Task<IActionResult> Stop()
        {
            var result = await _alpacaWebSocketService.StopSocketAsync();
            return Ok(new { message = result ? "Socket stopped." : "Socket was already stopped." });
        }

        /// <summary>
        /// Restarts the Alpaca WebSocket connection (with a 1s delay for cleanup).
        /// </summary>
        [HttpPost("restart")]
        public async Task<IActionResult> Restart()
        {
            await _alpacaWebSocketService.StopSocketAsync();
            await Task.Delay(1000); // small delay to ensure cleanup
            var result = await _alpacaWebSocketService.StartSocketAsync();
            return Ok(new { message = result ? "Socket restarted." : "Failed to restart socket." });
        }

        /// <summary>
        /// Forcefully disconnects the WebSocket session without restarting.
        /// </summary>
        [HttpPost("disconnect")]
        public async Task<IActionResult> Disconnect()
        {
            await _alpacaWebSocketService.DisconnectAsync();
            return Ok($"Disconnect Successful.");
        }

        /// <summary>
        /// Returns the current WebSocket connection state.
        /// </summary>
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { status = _alpacaWebSocketService.State.ToString() });
        }


        /// <summary>
        /// Searches for stock symbols or company names matching the query string.
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Query string 'q' is required.");

            var results = await _alpacaStockMarketService.SearchAsync(q);
            return Ok(results);
        }


        /// <summary>
        /// Searches for an alpaca asset by symbol. 
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> GetAsset([FromQuery] string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return BadRequest("Symbol is required.");

            var results = await _alpacaStockMarketService.GetAssetBySymbolAsync(symbol);
            return Ok(results);
        }

        /// <summary>
        /// Retrieves snapshot data for one or more symbols (latest quote, trade, etc).
        /// </summary>
        [HttpGet("snapshots")]
        public async Task<IActionResult> GetSnapshots([FromQuery] List<string> symbols)
        {
            if (symbols == null || !symbols.Any())
            {
                return BadRequest("At least one symbol must be provided.");
            }

            var snapshots = await _alpacaStockMarketService.GetSnapshotsAsync(symbols);

            if (snapshots == null)
            {
                return NotFound("No stock data found.");
            }

            return Ok(new { snapshots });
        }

        /// <summary>
        /// Retrieves historical bar (candlestick) data for a symbol, with optional start date.
        /// </summary>
        [HttpGet("{symbol}/historicaldata/{timeframe}")]
        public async Task<IActionResult> GetHistoricalBars(string symbol, string timeframe, [FromQuery] string start= "2024-01-01",  int limit = 1000)
        {
            var bars = await _alpacaStockMarketService.GetHistoricalBarsAsync(symbol, timeframe, start, limit); // synchronous
            if (bars == null)
            {
                return NotFound("bars not found.");
            }

            return Ok(new { bars });
        }

        /// <summary>
        /// Retrieves recent news for the specified symbol.
        /// </summary>
        [HttpGet("{symbol}/news")]
        public async Task<IActionResult> GetNews(string symbol, int limit)
        {
            var news = await _alpacaStockMarketService.GetNewsAsync(symbol, limit);
            if (news == null || !news.Any())
            {
                return NotFound("News not found.");
            }
            return Ok(new { news });
        }

        /// <summary>
        /// Retrieves a list of the most actively traded stocks.
        /// </summary>
        [HttpGet("most-actives")]
        public async Task<IActionResult> GetMostActives()
        {
            var data = await _alpacaStockMarketService.GetMostActivesAsync();
            return Ok(data);
        }

        /// <summary>
        /// Retrieves top market movers (gainers/losers) for a specified market type.
        /// </summary>
        [HttpGet("movers/{marketType}")]
        public async Task<IActionResult> GetMovers(string marketType)
        {
            var data = await _alpacaStockMarketService.GetMarketMoversAsync(marketType);
            return Ok(data);
        }

        /// <summary>
        /// Returns the latest trade for a given symbol from the WebSocket cache.
        /// </summary>
        [HttpGet("trade/{symbol}")]
        public IActionResult GetLatestTrade(string symbol)
        {
            if (_alpacaWebSocketService.LatestTrades.TryGetValue(symbol.ToUpper(), out var trade))
                return Ok(trade);

            return NotFound();
        }

        /// <summary>
        /// Returns the latest quote for a given symbol from the WebSocket cache.
        /// </summary>
        [HttpGet("quote/{symbol}")]
        public IActionResult GetLatestQuote(string symbol)
        {
            if (_alpacaWebSocketService.LatestQuotes.TryGetValue(symbol.ToUpper(), out var quote))
                return Ok(quote);

            return NotFound();
        }

        /// <summary>
        /// Returns the latest bar (candlestick) data for a given symbol from the WebSocket cache.
        /// </summary>
        [HttpGet("bar/{symbol}")]
        public IActionResult GetLatestBar(string symbol)
        {
            if (_alpacaWebSocketService.LatestBars.TryGetValue(symbol.ToUpper(), out var bar))
                return Ok(bar);

            return NotFound();
        }




        //[HttpPost("subscribe/all/{symbol}")]
        //public async Task<IActionResult> SubscribeToStock(string symbol)
        //{
        //    if (string.IsNullOrWhiteSpace(symbol))
        //        return BadRequest("Symbol is required.");

        //    _logger.LogInformation($"Received subscription request for {symbol}");
        //    await _alpacaWebSocketService.SubscribeToAllAsync(symbol);
        //    return Ok($"Subscribed to {symbol} (trades, quotes, bars)");

        //}
        //[HttpPost("subscribe/trades/{symbol}")]
        //public async Task<IActionResult> SubscribeToTrades(string symbol)
        //{
        //    await _alpacaWebSocketService.SubscribeToTradesAsync(symbol);
        //    return Ok($"Subscribed to trades for {symbol}");
        //}

        //[HttpPost("subscribe/quotes/{symbol}")]
        //public async Task<IActionResult> SubscribeToQuotes(string symbol)
        //{
        //    await _alpacaWebSocketService.SubscribeToQuotesAsync(symbol);
        //    return Ok($"Subscribed to quotes for {symbol}");
        //}

        //[HttpPost("subscribe/bars/{symbol}")]
        //public async Task<IActionResult> SubscribeToBars(string symbol)
        //{
        //    await _alpacaWebSocketService.SubscribeToBarsAsync(symbol);
        //    return Ok($"Subscribed to bars for {symbol}");
        //}

        //[HttpPost("unsubscribe/all/{symbol}")]
        //public async Task<IActionResult> UnsubscribeFromStock(string symbol)
        //{
        //    await _alpacaWebSocketService.UnsubscribeFromAllAsync(symbol);
        //    return Ok($"Unsubscribed from all updates for {symbol}");
        //}

        //[HttpPost("unsubscribe/trades/{symbol}")]
        //public async Task<IActionResult> UnsubscribeFromTrades(string symbol)
        //{
        //    await _alpacaWebSocketService.UnsubscribeFromTradesAsync(symbol);
        //    return Ok($"Unsubscribed from trades for {symbol}");
        //}

        //[HttpPost("unsubscribe/quotes/{symbol}")]
        //public async Task<IActionResult> UnsubscribeFromQuotes(string symbol)
        //{
        //    await _alpacaWebSocketService.UnsubscribeFromQuotesAsync(symbol);
        //    return Ok($"Unsubscribed from quotes for {symbol}");
        //}

        //[HttpPost("unsubscribe/bars/{symbol}")]
        //public async Task<IActionResult> UnsubscribeFromBars(string symbol)
        //{
        //    await _alpacaWebSocketService.UnsubscribeFromBarsAsync(symbol);
        //    return Ok($"Unsubscribed from bars for {symbol}");
        //}
    }
}
