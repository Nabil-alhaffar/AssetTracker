using System;
namespace AssetTracker.Models
{
	public class Portfolio
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public ICollection<Position> Positions { get; set; } = new List<Position>();
		//public double PNL = Assets.Sum(s=>s.PNL);
		//public double GetTotalValue()
		//{
		//	return Positions.Sum(p => p.GetMarketValue());
		//}
		//public double GetTotalProfitAndLoss()
		//{
		//	return Positions.Sum(p => p.GetProfitLoss());
		//}
        
		
        


        public Portfolio()
		{
		}
	}
}

