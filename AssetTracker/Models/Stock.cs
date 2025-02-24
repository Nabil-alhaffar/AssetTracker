﻿using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
    public class Stock
    {
        [Required]
        public string Symbol { get; set; }

        [Required]
        public string CompanyName { get; set; }
        public decimal? CurrentPrice { get; set; }
        public string? Country { get; set; }

        public double? MarketCap { get; set; }
        public double? High52Week { get; set; }
        public double? Low52Week { get; set; }
        public double? EPS { get; set; }
        public Status? StockStatus { get; set; }
        public Sector? StockSector { get; set; }
        public string? Exchange { get; set; }
        public double? MovingAverage50Day { get; set; }
        public double? MovingAverage200Day{get; set ;}
        public DateOnly? DividendDate { get; set; }
        public DateOnly? ExDividendDate { get; set; }
        public string? Description { get; set; }
        public string? LogoURL { get; set; }
        public double? AnalystTargetPrice { get; set; }
        public string? OfficialSite { get; set; }

        public GlobalQuote? Quote { get; set; } 

        public enum Sector
        {
            Energy,
            Materials,
            Industrials,
            ConsumerDiscretionary,
            ConsumerStaples,
            Healthcare,
            Financials,
            InformationTechnology,
            CommunicationServices,
            Utilities,
            RealEstate
        }
        public enum Status
        {
            Bullish,
            Bearish
        }

        public Stock()
		{
			
		}
	}
}

