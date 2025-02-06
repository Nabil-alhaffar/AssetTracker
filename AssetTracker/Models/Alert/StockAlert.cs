using System;
namespace AssetTracker.Models
{
    public class StockAlert
    {
        public string Symbol { get; set; }
        public AlertCondition Condition { get; set; }
        public Action TriggerAction { get; set; }

        public bool IsTriggered(decimal value)
        {
            return Condition.Type switch
            {
                AlertType.PriceAbove => value > Condition.Threshold,
                AlertType.PriceBelow => value < Condition.Threshold,
                _ => false
            };
        }
    }
}

