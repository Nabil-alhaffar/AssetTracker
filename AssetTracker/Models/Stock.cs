using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a stock with various financial and descriptive properties.
    /// </summary>
    public class Stock
    {
        /// <summary>
        /// The stock symbol (ticker).
        /// </summary>
        [Required]
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// The company name associated with the stock.
        /// </summary>
        [Required]
        public string CompanyName { get; set; } = null!;

        /// <summary>
        /// The current trading price of the stock.
        /// </summary>
        public decimal? CurrentPrice { get; set; }

        /// <summary>
        /// The country where the company is located.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Market capitalization value.
        /// </summary>
        public double? MarketCap { get; set; }

        /// <summary>
        /// Highest price of the stock in the last 52 weeks.
        /// </summary>
        public double? High52Week { get; set; }

        /// <summary>
        /// Lowest price of the stock in the last 52 weeks.
        /// </summary>
        public double? Low52Week { get; set; }

        /// <summary>
        /// Earnings per share.
        /// </summary>
        public double? EPS { get; set; }

        /// <summary>
        /// Current status of the stock (e.g., Bullish, Bearish).
        /// </summary>
        public Status? StockStatus { get; set; }

        /// <summary>
        /// Sector the stock belongs to.
        /// </summary>
        public Sector? StockSector { get; set; }

        /// <summary>
        /// Exchange on which the stock is traded.
        /// </summary>
        public string? Exchange { get; set; }

        /// <summary>
        /// 50-day moving average price.
        /// </summary>
        public double? MovingAverage50Day { get; set; }

        /// <summary>
        /// 200-day moving average price.
        /// </summary>
        public double? MovingAverage200Day { get; set; }

        /// <summary>
        /// Date when the next dividend is paid.
        /// </summary>
        public DateOnly? DividendDate { get; set; }

        /// <summary>
        /// Ex-dividend date.
        /// </summary>
        public DateOnly? ExDividendDate { get; set; }

        /// <summary>
        /// Description of the company or stock.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// URL to the company logo image.
        /// </summary>
        public string? LogoURL { get; set; }

        /// <summary>
        /// Analyst target price for the stock.
        /// </summary>
        public double? AnalystTargetPrice { get; set; }

        /// <summary>
        /// Official company website URL.
        /// </summary>
        public string? OfficialSite { get; set; }

        /// <summary>
        /// Global quote data associated with the stock.
        /// </summary>
        public GlobalQuote? Quote { get; set; }

        

        /// <summary>
        /// Defines stock status types.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
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