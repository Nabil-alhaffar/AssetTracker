﻿using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
	public class Portfolio
	{
		

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

