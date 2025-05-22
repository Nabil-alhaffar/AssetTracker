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
        private readonly AlpacaWebSocketService _alpacaWebSocketService;
        private readonly IAlpacaStockMarketService _alpacaStockMarketService;
        private readonly ILogger<AlpacaStockMarketController> _logger;

        public AlpacaStockMarketController(AlpacaWebSocketService alpacaService, IAlpacaStockMarketService alpacaStockMarketService, ILogger<AlpacaStockMarketController> logger)
        {
            _alpacaWebSocketService = alpacaService;
            _alpacaStockMarketService = alpacaStockMarketService;
            _logger = logger;
        }
        [HttpPost("start")]
        public async Task<IActionResult> Start()
        {
            var result = await _alpacaWebSocketService.StartSocketAsync();
            return Ok(new { message = result ? "Socket started." : "Socket was already running." });
        }

        [HttpPost("stop")]
        public async Task<IActionResult> Stop()
        {
            var result = await _alpacaWebSocketService.StopSocketAsync();
            return Ok(new { message = result ? "Socket stopped." : "Socket was already stopped." });
        }

        [HttpPost("restart")]
        public async Task<IActionResult> Restart()
        {
            await _alpacaWebSocketService.StopSocketAsync();
            await Task.Delay(1000); // small delay to ensure cleanup
            var result = await _alpacaWebSocketService.StartSocketAsync();
            return Ok(new { message = result ? "Socket restarted." : "Failed to restart socket." });
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { status = _alpacaWebSocketService._state.ToString() });
        }

        [HttpPost("subscribe/all/{symbol}")]
        public async Task<IActionResult> SubscribeToStock(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return BadRequest("Symbol is required.");

            _logger.LogInformation($"Received subscription request for {symbol}");
            await _alpacaWebSocketService.SubscribeToAllAsync(symbol);
            return Ok($"Subscribed to {symbol} (trades, quotes, bars)");
            //if (string.IsNullOrWhiteSpace(symbol))
            //    return BadRequest("Symbol is required.");
            //_logger.LogInformation($"Received subscription request for {symbol}");
            //await _alpacaWebSocketService.SubscribeAsync(symbol,true, true);
            //return Ok($"Subscribed to {symbol}");
        }
        [HttpPost("subscribe/trades/{symbol}")]
        public async Task<IActionResult> SubscribeToTrades(string symbol)
        {
            await _alpacaWebSocketService.SubscribeToTradesAsync(symbol);
            return Ok($"Subscribed to trades for {symbol}");
        }

        [HttpPost("subscribe/quotes/{symbol}")]
        public async Task<IActionResult> SubscribeToQuotes(string symbol)
        {
            await _alpacaWebSocketService.SubscribeToQuotesAsync(symbol);
            return Ok($"Subscribed to quotes for {symbol}");
        }

        [HttpPost("subscribe/bars/{symbol}")]
        public async Task<IActionResult> SubscribeToBars(string symbol)
        {
            await _alpacaWebSocketService.SubscribeToBarsAsync(symbol);
            return Ok($"Subscribed to bars for {symbol}");
        }

        [HttpGet("snapshot/{symbol}")]
        public async Task<IActionResult> GetSnapshot(string symbol)
        {
            var snapshot = await _alpacaStockMarketService.GetSnapshotAsync(symbol); // synchronous
            if (snapshot == null)
            {
                return NotFound("Stock not found.");
            }

            return Ok(new { snapshot});
        }

        [HttpGet("{symbol}/historicaldata/{timeframe}")]
        public async Task<IActionResult> GetHistoricalBars(string symbol, string timeframe, [FromQuery] string start)
        {
            var bars = await _alpacaStockMarketService.GetHistoricalBarsAsync(symbol,timeframe,start); // synchronous
            if (bars == null)
            {
                return NotFound("bars not found.");
            }

            return Ok(new { bars });
        }

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


        [HttpGet("trade/{symbol}")]
        public IActionResult GetLatestTrade(string symbol)
        {
            if (_alpacaWebSocketService.LatestTrades.TryGetValue(symbol.ToUpper(), out var trade))
                return Ok(trade);

            return NotFound();
        }

        [HttpGet("quote/{symbol}")]
        public IActionResult GetLatestQuote(string symbol)
        {
            if (_alpacaWebSocketService.LatestQuotes.TryGetValue(symbol.ToUpper(), out var quote))
                return Ok(quote);

            return NotFound();
        }

        [HttpGet("bar/{symbol}")]
        public IActionResult GetLatestBar(string symbol)
        {
            if (_alpacaWebSocketService.LatestBars.TryGetValue(symbol.ToUpper(), out var bar))
                return Ok(bar);

            return NotFound();
        }


        [HttpPost("unsubscribe/all/{symbol}")]
        public async Task<IActionResult> UnsubscribeFromStock(string symbol)
        {
            await _alpacaWebSocketService.UnsubscribeFromAllAsync(symbol);
            return Ok($"Unsubscribed from all updates for {symbol}");
        }

        [HttpPost("unsubscribe/trades/{symbol}")]
        public async Task<IActionResult> UnsubscribeFromTrades(string symbol)
        {
            await _alpacaWebSocketService.UnsubscribeFromTradesAsync(symbol);
            return Ok($"Unsubscribed from trades for {symbol}");
        }

        [HttpPost("unsubscribe/quotes/{symbol}")]
        public async Task<IActionResult> UnsubscribeFromQuotes(string symbol)
        {
            await _alpacaWebSocketService.UnsubscribeFromQuotesAsync(symbol);
            return Ok($"Unsubscribed from quotes for {symbol}");
        }

        [HttpPost("unsubscribe/bars/{symbol}")]
        public async Task<IActionResult> UnsubscribeFromBars(string symbol)
        {
            await _alpacaWebSocketService.UnsubscribeFromBarsAsync(symbol);
            return Ok($"Unsubscribed from bars for {symbol}");
        }

        [HttpPost("disconnect")]
        public async Task<IActionResult> Disconnect()
        {
            await _alpacaWebSocketService.DisconnectAsync();
            return Ok($"Disconnect Successful.");
        }
    }
}
