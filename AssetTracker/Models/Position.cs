using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
	public class Position
	{
        public int UserId { get; set; }
        public int Id { get; set; }
        public Stock Stock { get; set; }
        public int PortfolioId { get; set; }
        [Required]
        public DateTime DateEstablished { get; set; }
        [Required]
        public double Quantity { get; set; }
        [Required]
        public double AveragePurchasePrice { get; set; }
        public double? PNL => (Stock.CurrentPrice - AveragePurchasePrice) * Quantity;
        public string? StockSymbol => Stock.Symbol;
        public double? GetMarketValue()

        {
            return Stock.CurrentPrice * Quantity;
        }
        public double? GetProfitLoss()
        {
            return (Stock.CurrentPrice - AveragePurchasePrice) * Quantity;
        }

        public Position()
		{
		}
	}
}

