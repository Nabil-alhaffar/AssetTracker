﻿using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
	public sealed record User
	{
		[Required]
		public Guid UserId { get; set; } = Guid.NewGuid();

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

		public Portfolio Portfolio { get; set; } = new Portfolio();
        public List<Watchlist> Watchlists { get; set; } = new List<Watchlist>();  // Default to empty list if not provided


        public User()
		{
			Portfolio.UserId = this.UserId;
		}
		public User(string firstName, string lastName, Guid userId, string email)
		{
			this.FirstName = firstName;
			this.LastName = lastName;
			this.UserId = userId;
			this.Email = email;

			Portfolio = new Portfolio();
			Watchlists = new List<Watchlist>();
		}
	}
}

