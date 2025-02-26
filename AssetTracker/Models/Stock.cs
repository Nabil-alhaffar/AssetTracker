using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

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

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Sector
        {
            [EnumMember(Value = "Energy")]
            Energy,

            [EnumMember(Value = "Materials")]
            Materials,

            [EnumMember(Value = "Industrials")]
            Industrials,

            [EnumMember(Value = "Consumer_Discretionary")]
            ConsumerDiscretionary,

            [EnumMember(Value = "Consumer_Staples")]
            ConsumerStaples,

            [EnumMember(Value = "Healthcare")]
            Healthcare,

            [EnumMember(Value = "Financials")]
            Financials,

            [EnumMember(Value = "Information_Technology")]
            InformationTechnology,

            [EnumMember(Value = "Communication_Services")]
            CommunicationServices,

            [EnumMember(Value = "Utilities")]
            Utilities,

            [EnumMember(Value = "Real_Estate")]
            RealEstate
        }

        [JsonConverter(typeof(StringEnumConverter))]

        public enum Status
        {
            [EnumMember(Value = "Bullish")]
            Bullish,

            [EnumMember(Value = "Bearish")]
            Bearish
        }

        public Stock()
		{
			
		}
	}
}

