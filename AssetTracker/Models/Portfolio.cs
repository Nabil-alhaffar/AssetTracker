using System;
namespace AssetTracker.Models
{
	public class Portfolio
	{
		public static List<Stock> Assets { get; set; } = new List<Stock>();
		public double PNL = Assets.Sum(s=>s.PNL);
		public double TotalValue = Assets.Sum(s => s.MarketValue);
		
		public Portfolio()
		{
		}
	}
}

