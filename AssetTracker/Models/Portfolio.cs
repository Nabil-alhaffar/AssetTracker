using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
	public class Portfolio
	{
		
        //public double PNL = Assets.Sum(s=>s.PNL);
        //public double GetTotalValue()
        //{
        //	return Positions.Sum(p => p.GetMarketValue());
        //}
        //public double GetTotalProfitAndLoss()
        //{
        //	return Positions.Sum(p => p.GetProfitLoss());
        //}

        [Required]
        public int Id { get; set; }

        public int UserId { get; set; }
        //public User User { get; set; }

        public List<Position> Positions { get; set; } = new List<Position>();



        public Portfolio()
		{

			Positions = new List<Position>();
		}
	}
}

