using System;
namespace AssetTracker.Models
{
	public class WatchList
	{
		public int UserId { get; set; }
		public User User { get; set; }
		public string Name { get; set; }
		public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
		public WatchList()
		{
		}
	}
}

