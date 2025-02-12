using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
	public class Watchlist
	{

		public Guid UserId { get; set; }
		[Required]
		public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } // e.g., "Tech Stocks"
        public List<string> Symbols { get; set; } = new List<string>(); // Stock symbols
        //public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
		public Watchlist()
		{
		}
	}
}

