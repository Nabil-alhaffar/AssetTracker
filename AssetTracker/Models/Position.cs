using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace AssetTracker.Models
{
	public sealed record Position
	{
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid PositionId { get; set; } = Guid.NewGuid();
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public decimal AveragePurchasePrice { get; set; }
       
        public string Symbol { get; set; }
        public PositionType Type { get; set; }
        public decimal PositionRatio { get; private set; }
        public decimal CurrentPrice { get; set; } // Store real-time price here
        public decimal MarketValue => Quantity * CurrentPrice;
        public decimal TotalCost  => AveragePurchasePrice * Quantity;

        public void ComputePositionRatio(decimal totalPortfolioValue)
        {
            PositionRatio = totalPortfolioValue > 0 ? Math.Abs(MarketValue) / totalPortfolioValue : 0;
        }
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
        [JsonConverter(typeof(JsonStringEnumConverter))]

        public enum PositionType
        {
           [EnumMember(Value = "LONG")]
            Long,

           [EnumMember(Value = "SHORT")]
            Short,

            [EnumMember(Value = "OPTIONS")]
            Options,

            [EnumMember(Value = "FUTURES")]
            Futures
        }
        public Position()
		{

		}
	}
}

