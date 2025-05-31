using System;
using AssetTracker.Models.Finnhub;

namespace AssetTracker.Services.Interfaces
{
	public interface IFinnhubStockMarketService
	{
        Task<CompanyProfile?> GetCompanyProfileAsync(string symbol);
        Task<Quote?> GetQuoteAsync(string symbol);
        Task<Financials?> GetFinancialsAsync(string symbol);
        Task<List<FinancialReportFiling>?> GetFinancialsReportedAsync(string symbol, string from = "2024-01-01", string to = "2026-01-01");
    
        Task<EarningsCalendarResponse?> GetEarningsAsync(string symbol="" , string from = "2024-01-01", string to = "2026-01-01");
        //Task<StockNews[]?> GetCompanyNewsAsync(string symbol, string from, string to);
        Task<SocialSentiment?> GetSocialSentimentAsync(string symbol);
        Task<RecommendationTrend[]?> GetRecommendationTrendsAsync(string symbol);
        Task<PeersResponse?> GetPeersAsync(string symbol);
        Task<BasicFinancials?> GetBasicFinancialsAsync(string symbol);
    }
}

