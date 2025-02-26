using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using AssetTracker.Services;

namespace AssetTracker.Controllers
{
    [ApiController]
    [Route("api/alpaca")]
    public class AlpacaStockMarketController : ControllerBase
    {
        private readonly IAlpacaStockMarketService _alpacaService;
        private readonly ILogger<AlpacaStockMarketController> _logger;

        public AlpacaStockMarketController(IAlpacaStockMarketService alpacaService, ILogger<AlpacaStockMarketController> logger)
        {
            _alpacaService = alpacaService;
            _logger = logger;
        }

        [HttpPost("subscribe/{symbol}")]
        public async Task<IActionResult> SubscribeToStock(string symbol)
        {
            _logger.LogInformation($"Received subscription request for {symbol}");
            await _alpacaService.SubscribeToStockAsync(symbol, CancellationToken.None);
            return Ok($"Subscribed to {symbol}");
        }
    }
}
