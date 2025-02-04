using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
	public class WatchList
	{

		public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
		public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
		public WatchList()
		{
		}
	}
}

