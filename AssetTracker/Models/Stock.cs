using System;
namespace AssetTracker.Models
{
    public class Stock
    {
        public double CurrentPrice { get; set; }
        public string Country { get; set; }
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public double MarketCap { get; set; }
        public double High52Week { get; set; }
        public double Low52Week { get; set; }
        public double EPS { get; set; }
        public Status StockStatus { get; set; }
        public Sector StockSector { get; set; }
        public string Exchange { get; set; }
        public double MovingAverage50Day { get; set; }
        public double MovingAverage200Day{get; set ;}
        public DateOnly DividendDate { get; set; }
        public DateOnly ExDividendDate { get; set; }
        public string Description { get; set; }
        public string LogoURL { get; set; }
        public double AnalystTargetPrice { get; set; }
        public string OfficialSite { get; set; }
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


        //      public double Quantity { get; set; }
        //public double AveragePurchasePrice { get; set; }
        //public DateTime DateEstablished { get; set; }
        //      public double MarketValue => Quantity * CurrentPrice;


        //public double PNL=>(CurrentPrice-AveragePurchasePrice)*Quantity;
        public Stock()
		{
			
		}
	}
}

