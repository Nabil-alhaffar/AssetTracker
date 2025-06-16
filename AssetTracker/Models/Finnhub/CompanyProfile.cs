using System;

namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents a company's profile information as provided by Finnhub.
    /// </summary>
    public sealed record CompanyProfile
    {
        /// <summary>
        /// The full name of the company.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The stock ticker symbol of the company.
        /// </summary>
        public string Ticker { get; set; } = null!;

        /// <summary>
        /// The stock exchange where the company is listed.
        /// </summary>
        public string Exchange { get; set; } = null!;

        /// <summary>
        /// The company's official website URL.
        /// </summary>
        public string Weburl { get; set; } = null!;

        //public string Industry { get; set; } // Commented out as unused.

        /// <summary>
        /// The URL to the company's logo image.
        /// </summary>
        public string Logo { get; set; } = null!;

        /// <summary>
        /// The country where the company is headquartered.
        /// </summary>
        public string Country { get; set; } = null!;

        /// <summary>
        /// The date of the company's initial public offering (IPO).
        /// </summary>
        public string IPO { get; set; } = null!;

        /// <summary>
        /// Contact phone number of the company.
        /// </summary>
        public string Phone { get; set; } = null!;

        /// <summary>
        /// The industry classification as defined by Finnhub.
        /// </summary>
        public string FinnhubIndustry { get; set; } = null!;

        /// <summary>
        /// The currency in which the company reports its financials.
        /// </summary>
        public string Currency { get; set; } = null!;

        /// <summary>
        /// The total market capitalization of the company.
        /// </summary>
        public decimal MarketCapitalization { get; set; }

        /// <summary>
        /// The total number of outstanding shares.
        /// </summary>
        public decimal ShareOutstanding { get; set; }
    }
}