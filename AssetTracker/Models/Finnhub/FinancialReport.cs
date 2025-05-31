using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AssetTracker.Models.Finnhub
{
	public class FinancialReport
	{
        [JsonPropertyName("ic")]
        public List<FinancialMetric> IncomeStatement { get; set; }

        [JsonPropertyName("bs")]
        public List<FinancialMetric> BalanceSheet { get; set; }

        [JsonPropertyName("cf")]
        public List<FinancialMetric> CashFlow { get; set; }
    }
}

