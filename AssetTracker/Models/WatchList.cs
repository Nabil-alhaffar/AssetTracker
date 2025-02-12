using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
	public class Watchlist
	{

		public Guid UserId { get; set; }
        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } // e.g., "Tech Stocks"
        public List<string> Symbols { get; set; } = new List<string>(); // Stock symbols
        //public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
		public Watchlist()
		{
		}
	}
}

