using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
	public sealed record Portfolio
	{


        //[Required]
        //public Guid PortfolioId { get; set; } = Guid.NewGuid();
        //public User User { get; set; }

        public Guid UserId { get; set; }
        public Dictionary<string, Position> Positions { get; set; } = new ();
		public decimal AvailableFunds { get; set; } = 0; 


        public Portfolio()
		{

			Positions = new Dictionary<string, Position>();
			 
		}
	}
}

