using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
	public class Position
	{

        public Guid UserId { get; set; }
        public Guid PositionId { get; set; } = Guid.NewGuid();
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public decimal AveragePurchasePrice { get; set; }
       
        public string Symbol { get; set; }
        public PositionType Type { get; set; }

        //public Stock Stock { get; set; }
        //public Guid PortfolioId { get; set; }
        //[Required]
        //public DateTime DateEstablished { get; set; }
        //public decimal? TotalCost  => AveragePurchasePrice * Quantity;
        //public decimal? PNL => (Stock.CurrentPrice - AveragePurchasePrice) * Quantity;
        //public string? StockSymbol { get; set; }
        //public decimal? MarketValue => Quantity * Stock.CurrentPrice;
        //public decimal? GetProfitLoss()
        //{
        //    return (Stock.CurrentPrice - AveragePurchasePrice) * Quantity;
        //}
        public enum PositionType
        {
            Long,
            Short,
            Options,
            Futures
        }
        public Position()
		{
		}
	}
}

