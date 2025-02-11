using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{
    public class Order
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        [Required]
        public string Symbol { get; set; }
        [Required]
        public decimal Quantity { get; set; }

        public decimal Price { get; set; }
        public OrderType Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public enum OrderType
    {
        Buy,
        Sell,
        Short,
        CloseShort
    }


}

