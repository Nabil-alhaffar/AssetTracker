using System;
using AssetTracker.Models.Finnhub;

namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Interface for accessing stock market data from the Finnhub API.
    /// </summary>
    public interface IFinnhubStockMarketService
    {
        /// <summary>
        /// Gets the company profile for the specified symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The company profile, or null if not found.</returns>
        Task<CompanyProfile?> GetCompanyProfileAsync(string symbol);

        /// <summary>
        /// Gets the current quote (price and volume data) for the specified symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The quote data, or null if not found.</returns>
        Task<Quote?> GetQuoteAsync(string symbol);

        /// <summary>
        /// Gets detailed financial metrics for a company.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The financials, or null if not found.</returns>
        Task<Financials?> GetFinancialsAsync(string symbol);

        /// <summary>
        /// Gets reported financial filings for a company within a given date range.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <param name="from">The start date in YYYY-MM-DD format.</param>
        /// <param name="to">The end date in YYYY-MM-DD format.</param>
        /// <returns>A list of financial report filings, or null if none found.</returns>
        Task<List<FinancialReportFiling>?> GetFinancialsReportedAsync(string symbol, string from = "2024-01-01", string to = "2026-01-01");

        /// <summary>
        /// Gets earnings calendar data for a company within a specified range.
        /// </summary>
        /// <param name="symbol">The stock symbol. Leave empty to fetch for all companies.</param>
        /// <param name="from">The start date in YYYY-MM-DD format.</param>
        /// <param name="to">The end date in YYYY-MM-DD format.</param>
        /// <returns>The earnings calendar response, or null if not found.</returns>
        Task<EarningsCalendarResponse?> GetEarningsAsync(string symbol = "", string from = "2024-01-01", string to = "2026-01-01");

        /// <summary>
        /// Gets social sentiment data for a company.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The social sentiment data, or null if not found.</returns>
        Task<SocialSentiment?> GetSocialSentimentAsync(string symbol);

        /// <summary>
        /// Gets analyst recommendation trends for a company.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>An array of recommendation trends, or null if not found.</returns>
        Task<RecommendationTrend[]?> GetRecommendationTrendsAsync(string symbol);

        /// <summary>
        /// Gets a list of peer companies for the specified symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The peers response, or null if not found.</returns>
        Task<PeersResponse?> GetPeersAsync(string symbol);

        /// <summary>
        /// Gets basic financial metrics for a company.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The basic financials, or null if not found.</returns>
        Task<BasicFinancials?> GetBasicFinancialsAsync(string symbol);
    }
}