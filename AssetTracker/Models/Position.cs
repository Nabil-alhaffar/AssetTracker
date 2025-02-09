using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
	public class Position
	{
        [Required]
        public DateTime DateEstablished { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public decimal AveragePurchasePrice { get; set; }
        public Guid UserId { get; set; }
        public Guid PositionId { get; set; } = Guid.NewGuid();
        public Stock Stock { get; set; }
        public Guid PortfolioId { get; set; }
        public decimal? TotalCost  => AveragePurchasePrice * Quantity;
        public decimal? PNL => (Stock.CurrentPrice - AveragePurchasePrice) * Quantity;
        public string? StockSymbol => Stock.Symbol;
        public decimal? MarketValue => Quantity * Stock.CurrentPrice;
        public decimal? GetProfitLoss()
        {
            return (Stock.CurrentPrice - AveragePurchasePrice) * Quantity;
        }

        public Position()
		{
		}
	}
}

