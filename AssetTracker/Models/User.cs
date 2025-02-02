using System;
namespace AssetTracker.Models
{
	public class User
	{
		public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
		public string Email { get; set; }
		public int UserId { get; set; }
		public Portfolio Portfolio { get; set; }
        // A user can have multiple watchlists
        public ICollection<WatchList> Watchlists { get; set; } = new List<WatchList>();

        public User()
		{

		}
	}
}

