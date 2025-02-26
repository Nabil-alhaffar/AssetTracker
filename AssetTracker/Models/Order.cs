using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace AssetTracker.Models
{
    public sealed record Order
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

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderType
    {
        [EnumMember(Value = "BUY")]
        Buy,


        [EnumMember(Value = "SELL")]
        Sell,

        [EnumMember(Value = "SHORT")]
        Short,
        [EnumMember(Value = "CLOSE_SHORT")]

        CloseShort
    }


}

