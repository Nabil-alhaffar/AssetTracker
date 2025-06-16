using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents a financial report containing income statement, balance sheet, and cash flow metrics.
    /// </summary>
    public sealed record FinancialReport
    {
        /// <summary>
        /// List of financial metrics in the income statement.
        /// </summary>
        [JsonPropertyName("ic")]
        public List<FinancialMetric> IncomeStatement { get; set; } = new();

        /// <summary>
        /// List of financial metrics in the balance sheet.
        /// </summary>
        [JsonPropertyName("bs")]
        public List<FinancialMetric> BalanceSheet { get; set; } = new();

        /// <summary>
        /// List of financial metrics in the cash flow statement.
        /// </summary>
        [JsonPropertyName("cf")]
        public List<FinancialMetric> CashFlow { get; set; } = new();
    }
}