using System;
namespace AssetTracker.Models
{
	public class Portfolio
	{

		public static List<Position> Positions { get; set; } = new List<Position>();
		//public double PNL = Assets.Sum(s=>s.PNL);
		public double GetTotalValue()
		{
			return Positions.Sum(p => p.GetMarketValue());
		}
		public double GetTotalProfitAndLoss()
		{
			return Positions.Sum(p => p.GetProfitLoss());
		}
        //public void AddPosition(Position position)
        //{
        //    Positions.Add(position);
        //}

        //// Remove a position from the portfolio by stock symbol
        //public void RemovePosition(string stockSymbol)
        //{
        //    var position = Positions.FirstOrDefault(p => p.Stock.Symbol == stockSymbol);
        //    if (position != null)
        //    {
        //        Positions.Remove(position);
        //    }
		
        


        public Portfolio()
		{
		}
	}
}

