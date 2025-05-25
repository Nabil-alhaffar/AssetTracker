using System;
using AssetTracker.Services;
using AssetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracker.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FinnhubController : ControllerBase
    {
        private readonly IFinnhubStockMarketService _finnhubService;

        public FinnhubController(IFinnhubStockMarketService finnhubService)
        {
            _finnhubService = finnhubService;
        }

        [HttpGet("profile/{symbol}")]
        public async Task<IActionResult> GetProfile(string symbol)
        {
            var profile = await _finnhubService.GetCompanyProfileAsync(symbol);
            return profile is not null ? Ok(profile) : NotFound();
        }

        //[HttpGet("quote/{symbol}")]
        //public async Task<IActionResult> GetQuote(string symbol)
        //{
        //    var quote = await _finnhubService.GetQuoteAsync(symbol);
        //    return quote is not null ? Ok(quote) : NotFound();
        //}

        [HttpGet("financials/{symbol}")]
        public async Task<IActionResult> GetFinancials(string symbol)
        {
            var data = await _finnhubService.GetFinancialsAsync(symbol);
            return data is not null ? Ok(data) : NotFound();
        }

        [HttpGet("earnings/{symbol}")]
        public async Task<IActionResult> GetEarnings(string symbol)
        {
            var earnings = await _finnhubService.GetEarningsAsync(symbol);
            return earnings is not null ? Ok(earnings) : NotFound();
        }

        //[HttpGet("news/{symbol}")]
        //public async Task<IActionResult> GetCompanyNews(string symbol, string from, string to)
        //{
        //    var news = await _finnhubService.GetCompanyNewsAsync(symbol, from, to);
        //    return news is not null ? Ok(news) : NotFound();
        //}

        [HttpGet("sentiment/{symbol}")]
        public async Task<IActionResult> GetSocialSentiment(string symbol)
        {
            var sentiment = await _finnhubService.GetSocialSentimentAsync(symbol);
            return sentiment is not null ? Ok(sentiment) : NotFound();
        }

        [HttpGet("recommendations/{symbol}")]
        public async Task<IActionResult> GetRecommendations(string symbol)
        {
            var recs = await _finnhubService.GetRecommendationTrendsAsync(symbol);
            return recs is not null ? Ok(recs) : NotFound();
        }

        [HttpGet("peers/{symbol}")]
        public async Task<IActionResult> GetPeers(string symbol)
        {
            var peers = await _finnhubService.GetPeersAsync(symbol);
            return peers is not null ? Ok(peers) : NotFound();
        }

        [HttpGet("basic-financials/{symbol}")]
        public async Task<IActionResult> GetBasicFinancials(string symbol)
        {
            var data = await _finnhubService.GetBasicFinancialsAsync(symbol);
            return data is not null ? Ok(data) : NotFound();
        }
    }
}

