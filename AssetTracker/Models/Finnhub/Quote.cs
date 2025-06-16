using System;
namespace AssetTracker.Models.Finnhub
{
    /// <summary>
    /// Represents a stock quote with key price and timestamp information. 
    /// </summary>
    public class Quote
    {

        /// <summary>
        /// Gets or sets the current price.
        /// </summary>
        public decimal C { get; set; }  

        /// <summary>
        /// Gets or sets the high price of the day. 
        /// </summary>
        public decimal H { get; set; }

        /// <summary>
        /// Gets or sets the low price of the day. 
        /// </summary>
        public decimal L { get; set; }


        /// <summary>
        /// Gets or sets the opening price,
        /// </summary>
        public decimal O { get; set; }  

        /// <summary>
        /// Gets or sets the previous closing price.
        /// </summary>
        public decimal Pc { get; set; }


        /// <summary>
        /// Gets or sets the timestamp of the quote in Unix time.
        /// </summary>
        public decimal T { get; set; } 

    }
}

