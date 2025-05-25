using System;
using AssetTracker.Models.Finnhub;

namespace AssetTracker.Services.Interfaces
{
	public interface IFinnhubStockMarketService
	{
        Task<CompanyProfile?> GetCompanyProfileAsync(string symbol);
        //Task<Quote?> GetQuoteAsync(string symbol);
        Task<Financials?> GetFinancialsAsync(string symbol);
        Task<EarningsCalendar?> GetEarningsAsync(string symbol);
        //Task<StockNews[]?> GetCompanyNewsAsync(string symbol, string from, string to);
        Task<SocialSentiment?> GetSocialSentimentAsync(string symbol);
        Task<RecommendationTrend[]?> GetRecommendationTrendsAsync(string symbol);
        Task<PeersResponse?> GetPeersAsync(string symbol);
        Task<BasicFinancials?> GetBasicFinancialsAsync(string symbol);
    }
}

