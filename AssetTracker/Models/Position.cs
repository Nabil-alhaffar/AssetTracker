using System;
namespace AssetTracker.Models
{
	public class Position
	{
        public int Id { get; set; }
        public Stock Stock { get; set; }
        public double Quantity { get; set; }
        public double AveragePurchasePrice { get; set; }
        public DateTime DateEstablished { get; set; }
        public string StockSymbol => Stock.Symbol;
        public double GetMarketValue()

        {
            return Stock.CurrentPrice * Quantity;
        }
        public double GetProfitLoss()
        {
            return (Stock.CurrentPrice - AveragePurchasePrice) * Quantity;
        }

        public Position()
		{
		}
	}
}

