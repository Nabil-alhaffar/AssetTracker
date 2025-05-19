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
        private readonly AlpacaWebSocketService _alpacaService;

        private readonly ILogger<AlpacaStockMarketController> _logger;

        public AlpacaStockMarketController(AlpacaWebSocketService alpacaService, ILogger<AlpacaStockMarketController> logger)
        {
            _alpacaService = alpacaService;
            _logger = logger;
        }

        [HttpPost("subscribe/{symbol}")]
        public async Task<IActionResult> SubscribeToStock(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return BadRequest("Symbol is required.");
            _logger.LogInformation($"Received subscription request for {symbol}");
            await _alpacaService.SubscribeAsync(symbol,true, true);
            return Ok($"Subscribed to {symbol}");
        }
        [HttpGet("trade/{symbol}")]
        public IActionResult GetLatestTrade(string symbol)
        {
            if (_alpacaService.LatestTrades.TryGetValue(symbol.ToUpper(), out var trade))
                return Ok(trade);

            return NotFound();
        }

        [HttpGet("quote/{symbol}")]
        public IActionResult GetLatestQuote(string symbol)
        {
            if (_alpacaService.LatestQuotes.TryGetValue(symbol.ToUpper(), out var quote))
                return Ok(quote);

            return NotFound();
        }

        [HttpGet("bar/{symbol}")]
        public IActionResult GetLatestBar(string symbol)
        {
            if (_alpacaService.LatestBars.TryGetValue(symbol.ToUpper(), out var bar))
                return Ok(bar);

            return NotFound();
        }
    }
}
