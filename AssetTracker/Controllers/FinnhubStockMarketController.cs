using System;
using AssetTracker.Services;
using AssetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracker.Controllers
{
    // Indicates that this is an API controller and enables automatic model validation
    [ApiController]
    [Route("api/[controller]")] // Routes will be prefixed with "api/finnhub"
    public class FinnhubController : ControllerBase
    {
        // Service interface to handle communication with the Finnhub API
        private readonly IFinnhubStockMarketService _finnhubService;

        // Constructor injecting the Finnhub service
        public FinnhubController(IFinnhubStockMarketService finnhubService)
        {
            _finnhubService = finnhubService;
        }

        /// <summary>
        /// Get company profile data for a given symbol.
        /// </summary>
        [HttpGet("profile/{symbol}")]
        public async Task<IActionResult> GetProfile(string symbol)
        {
            var profile = await _finnhubService.GetCompanyProfileAsync(symbol);
            return profile is not null ? Ok(profile) : NotFound();
        }

        /// <summary>
        /// Get real-time stock quote for a given symbol.
        /// </summary>
        [HttpGet("quote/{symbol}")]
        public async Task<IActionResult> GetQuote(string symbol)
        {
            var quote = await _finnhubService.GetQuoteAsync(symbol);
            return quote is not null ? Ok(quote) : NotFound();
        }

        // Premium endpoint - commented out because Finnhub may require a paid plan
        /*
        [HttpGet("financials/{symbol}")]
        public async Task<IActionResult> GetFinancials(string symbol)
        {
            var data = await _finnhubService.GetFinancialsAsync(symbol);
            return data is not null ? Ok(data) : NotFound();
        }
        */

        /// <summary>
        /// Get reported financials for a company in a date range.
        /// </summary>
        /// <param name="symbol">Stock symbol</param>
        /// <param name="start">Start date (default: 2024-01-01)</param>
        /// <param name="end">End date (default: 2026-01-01)</param>
        [HttpGet("financials-reported/{symbol}")]
        public async Task<IActionResult> GetFinancialsReported(
            string symbol,
            [FromQuery] string start = "2024-01-01",
            [FromQuery] string end = "2026-01-01")
        {
            var data = await _finnhubService.GetFinancialsReportedAsync(symbol, start, end);
            return data != null ? Ok(data) : NotFound();
        }

        /// <summary>
        /// Get historical earnings reports for a company.
        /// </summary>
        /// <param name="symbol">Stock symbol (optional)</param>
        /// <param name="from">Start date</param>
        /// <param name="to">End date</param>
        [HttpGet("earnings/{symbol}")]
        public async Task<IActionResult> GetEarnings(
            string symbol = "",
            [FromQuery] string from = "2024-01-01",
            [FromQuery] string to = "2026-01-01")
        {
            var earnings = await _finnhubService.GetEarningsAsync(symbol, from, to);
            return earnings is not null ? Ok(earnings) : NotFound();
        }

        // Another premium endpoint (e.g., for company news), commented out
        /*
        [HttpGet("news/{symbol}")]
        public async Task<IActionResult> GetCompanyNews(string symbol, string from, string to)
        {
            var news = await _finnhubService.GetCompanyNewsAsync(symbol, from, to);
            return news is not null ? Ok(news) : NotFound();
        }
        */

        /// <summary>
        /// Get social sentiment (e.g., Reddit, Twitter) for a given symbol.
        /// </summary>
        [HttpGet("sentiment/{symbol}")]
        public async Task<IActionResult> GetSocialSentiment(string symbol)
        {
            var sentiment = await _finnhubService.GetSocialSentimentAsync(symbol);
            return sentiment is not null ? Ok(sentiment) : NotFound();
        }

        /// <summary>
        /// Get analyst recommendation trends for a stock symbol.
        /// </summary>
        [HttpGet("recommendations/{symbol}")]
        public async Task<IActionResult> GetRecommendations(string symbol)
        {
            var recs = await _finnhubService.GetRecommendationTrendsAsync(symbol);
            return recs is not null ? Ok(recs) : NotFound();
        }

        /// <summary>
        /// Get peer companies for a given stock symbol.
        /// </summary>
        [HttpGet("peers/{symbol}")]
        public async Task<IActionResult> GetPeers(string symbol)
        {
            var peers = await _finnhubService.GetPeersAsync(symbol);
            return peers is not null ? Ok(peers) : NotFound();
        }

        /// <summary>
        /// Get basic financial metrics (e.g., P/E, EPS, etc.) for a company.
        /// </summary>
        [HttpGet("basic-financials/{symbol}")]
        public async Task<IActionResult> GetBasicFinancials(string symbol)
        {
            var data = await _finnhubService.GetBasicFinancialsAsync(symbol);
            return data is not null ? Ok(data) : NotFound();
        }
    }
}