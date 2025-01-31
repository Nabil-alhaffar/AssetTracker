using System;
namespace AssetTracker.Models
{
	public class Stock
	{
        public float CurrentPrice { get; set; }
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public double MarketCap { get; set; }
        public enum Status { Bullish, Bearish }
        public double High52Week { get; set; }
        public double Low52Week { get; set; }
        public double EPS { get; set; }
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
            Real, Estate
        }

        public double Quantity { get; set; }
		public double PurchasePrice { get; set; }
		public DateTime DateEstablished { get; set; }
		
        public double PNL=>(CurrentPrice-PurchasePrice)*Quantity;
        public Stock()
		{
			
		}
	}
}

