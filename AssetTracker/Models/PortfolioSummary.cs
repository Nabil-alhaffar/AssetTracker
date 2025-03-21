using System;
namespace AssetTracker.Models
{
    public sealed record PortfolioSummary
    {
        public decimal MarketValue { get; set; }
        public decimal Cost { get; set; }
        public decimal NetAccountValue { get; set; }
        public decimal CashBalance { get; set; }
        public decimal DayPNL { get; set; }
        public decimal DayReturnPercentage { get; set; }
        public decimal OpenPNL { get; set; }
        public decimal OpenReturnPercentage { get; set; }
    }


  
}

