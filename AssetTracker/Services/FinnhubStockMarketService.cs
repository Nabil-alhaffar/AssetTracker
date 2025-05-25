using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using AssetTracker.Models.Finnhub;
using AssetTracker.Services.Interfaces;
namespace AssetTracker.Services
{


    public class FinnhubService: IFinnhubStockMarketService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://finnhub.io/api/v1/";

        public FinnhubService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["Finnhub:ApiKey"];
        }

        private async Task<T?> GetAsync<T>(string endpoint)
        {
            var url = $"{BaseUrl}{endpoint}&token={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return default;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public Task<CompanyProfile?> GetCompanyProfileAsync(string symbol) =>
            GetAsync<CompanyProfile>($"stock/profile2?symbol={symbol}");

        //public Task<Quote?> GetQuoteAsync(string symbol) =>
        //    GetAsync<Quote>($"quote?symbol={symbol}");

        public Task<Financials?> GetFinancialsAsync(string symbol) =>
            GetAsync<Financials>($"stock/financials?symbol={symbol}&statement=bs&freq=annual");

        public Task<EarningsCalendar?> GetEarningsAsync(string symbol) =>
            GetAsync<EarningsCalendar>($"calendar/earnings?symbol={symbol}");

        //public Task<StockNews[]> GetCompanyNewsAsync(string symbol, string from, string to) =>
        //    GetAsync<StockNews[]>($"company-news?symbol={symbol}&from={from}&to={to}");

        public Task<SocialSentiment?> GetSocialSentimentAsync(string symbol) =>
            GetAsync<SocialSentiment>($"stock/social-sentiment?symbol={symbol}");

        public Task<RecommendationTrend[]?> GetRecommendationTrendsAsync(string symbol) =>
            GetAsync<RecommendationTrend[]>($"stock/recommendation?symbol={symbol}");

        public Task<PeersResponse?> GetPeersAsync(string symbol) =>
            GetAsync<PeersResponse>($"stock/peers?symbol={symbol}");

        public Task<BasicFinancials?> GetBasicFinancialsAsync(string symbol) =>
            GetAsync<BasicFinancials>($"stock/metric?symbol={symbol}&metric=all");
    }

}

