﻿using System;
namespace AssetTracker.Models
{
    public sealed record PortfolioSummary
    {
        public decimal TotalMarketValue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal PNL { get; set; }
        public decimal ReturnPercentage { get; set; }
    }


  
}

