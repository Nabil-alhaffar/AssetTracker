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
    /// <summary>
    /// Service to interact with Finnhub API for stock market data.
    /// </summary>
    public class FinnhubService : IFinnhubStockMarketService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://finnhub.io/api/v1/";

        public FinnhubService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["Finnhub:ApiKey"];
        }

        /// <summary>
        /// Sends a GET request to the specified Finnhub API endpoint and deserializes the JSON response.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into.</typeparam>
        /// <param name="endpoint">The Finnhub API endpoint (excluding base URL and token).</param>
        /// <returns>The deserialized object of type <typeparamref name="T"/> if successful; otherwise, default.</returns>
        private async Task<T?> GetAsync<T>(string endpoint)
        {
            var url = $"{BaseUrl}{endpoint}&token={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return default;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        /// <summary>
        /// Retrieves company profile information.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A <see cref="CompanyProfile"/> object.</returns>
        public Task<CompanyProfile?> GetCompanyProfileAsync(string symbol) =>
            GetAsync<CompanyProfile>($"stock/profile2?symbol={symbol}");

        /// <summary>
        /// Retrieves current stock quote data.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A <see cref="Quote"/> object.</returns>
        public Task<Quote?> GetQuoteAsync(string symbol) =>
            GetAsync<Quote>($"quote?symbol={symbol}");

        /// <summary>
        /// Retrieves company financials (annual balance sheet).
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A <see cref="Financials"/> object.</returns>
        public Task<Financials?> GetFinancialsAsync(string symbol) =>
            GetAsync<Financials>($"stock/financials?symbol={symbol}&statement=bs&freq=annual");

        /// <summary>
        /// Retrieves reported financial filings between two dates.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <param name="from">Start date (yyyy-MM-dd).</param>
        /// <param name="to">End date (yyyy-MM-dd).</param>
        /// <returns>A list of <see cref="FinancialReportFiling"/> objects.</returns>
        public async Task<List<FinancialReportFiling>?> GetFinancialsReportedAsync(string symbol, string from = "2020-01-01", string to = "2025-12-31")
        {
            var result = await GetAsync<FinancialsReportedResponse>($"stock/financials-reported?symbol={symbol}&from={from}&to={to}");
            return result?.Data;
        }

        /// <summary>
        /// Retrieves earnings calendar data for a stock or date range.
        /// </summary>
        /// <param name="symbol">The stock symbol (optional).</param>
        /// <param name="from">Start date (yyyy-MM-dd).</param>
        /// <param name="to">End date (yyyy-MM-dd).</param>
        /// <returns>An <see cref="EarningsCalendarResponse"/> object.</returns>
        public Task<EarningsCalendarResponse?> GetEarningsAsync(string symbol = "", string from = "2024-01-01", string to = "2026-01-01") =>
            GetAsync<EarningsCalendarResponse>($"calendar/earnings?from={from}&to={to}&symbol={symbol}");

        /// <summary>
        /// Retrieves social sentiment data for a stock.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A <see cref="SocialSentiment"/> object.</returns>
        public Task<SocialSentiment?> GetSocialSentimentAsync(string symbol) =>
            GetAsync<SocialSentiment>($"stock/social-sentiment?symbol={symbol}");

        /// <summary>
        /// Retrieves analyst recommendation trends.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>An array of <see cref="RecommendationTrend"/> objects.</returns>
        public Task<RecommendationTrend[]?> GetRecommendationTrendsAsync(string symbol) =>
            GetAsync<RecommendationTrend[]>($"stock/recommendation?symbol={symbol}");

        /// <summary>
        /// Retrieves a list of peer companies for the given symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A <see cref="PeersResponse"/> object containing peer symbols.</returns>
        public Task<PeersResponse?> GetPeersAsync(string symbol) =>
            GetAsync<PeersResponse>($"stock/peers?symbol={symbol}");

        /// <summary>
        /// Retrieves basic financial metrics for a company.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A <see cref="BasicFinancials"/> object containing financial metrics.</returns>
        public Task<BasicFinancials?> GetBasicFinancialsAsync(string symbol) =>
            GetAsync<BasicFinancials>($"stock/metric?symbol={symbol}&metric=all");
    }
}