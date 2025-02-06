using System;
namespace AssetTracker.Models
{
    public class AlertCondition
    {
        public decimal Threshold { get; set; }
        public AlertType Type { get; set; }
    }

    public enum AlertType
    {
        PriceAbove,
        PriceBelow,
        SMA,
        EMA,
        MACD,
        RSI,
        BollingerBands
    }
}

