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

        [HttpPost("subscribe/{symbol}")]
        public async Task<IActionResult> SubscribeToStock(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return BadRequest("Symbol is required.");
            _logger.LogInformation($"Received subscription request for {symbol}");
            await _alpacaWebSocketService.SubscribeAsync(symbol,true, true);
            return Ok($"Subscribed to {symbol}");
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
        public async Task <IActionResult> GetNews (string symbol, int limit)
        {
            var news = _alpacaStockMarketService.GetNewsAsync(symbol, limit);
            if (news == null)
            {
                return NotFound("news not found");
            }
            return Ok(new {news});
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
    }
}
