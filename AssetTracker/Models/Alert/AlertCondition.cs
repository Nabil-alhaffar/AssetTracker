using System;
using System.Collections.Generic;
using AssetTracker.Models.Enums;
namespace AssetTracker.Models.Alert
{
    /// <summary>
    /// Represents a condition that can trigger an alert.
    /// </summary>
    public class AlertCondition
    {
        /// <summary>
        /// Gets or sets the type of alert condition.
        /// </summary>
        public AlertType Type { get; set; }

        /// <summary>
        /// Gets or sets the threshold value for the condition.
        /// </summary>
        public decimal Threshold { get; set; }

        /// <summary>
        /// Gets or sets the comparison operator for the condition.
        /// </summary>
        public ComparisonOperator Operator { get; set; } = ComparisonOperator.GreaterThan;

        /// <summary>
        /// Gets or sets additional parameters for complex conditions.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();

        /// <summary>
        /// Checks if the condition is triggered based on the current value.
        /// </summary>
        /// <param name="currentValue">The current value to check against.</param>
        /// <returns>True if the condition is triggered; otherwise, false.</returns>
        public bool IsTriggered(decimal currentValue)
        {
            return Operator switch
            {
                ComparisonOperator.GreaterThan => currentValue > Threshold,
                ComparisonOperator.GreaterThanOrEqual => currentValue >= Threshold,
                ComparisonOperator.LessThan => currentValue < Threshold,
                ComparisonOperator.LessThanOrEqual => currentValue <= Threshold,
                ComparisonOperator.Equal => currentValue == Threshold,
                ComparisonOperator.NotEqual => currentValue != Threshold,
                ComparisonOperator.Between => IsBetween(currentValue),
                _ => false
            };
        }

        /// <summary>
        /// Checks if the value is between the specified range.
        /// </summary>
        /// <param name="currentValue">The current value to check.</param>
        /// <returns>True if the value is within the range; otherwise, false.</returns>
        private bool IsBetween(decimal currentValue)
        {
            if (Parameters.TryGetValue("MinValue", out var minValueObj) && 
                Parameters.TryGetValue("MaxValue", out var maxValueObj))
            {
                if (decimal.TryParse(minValueObj.ToString(), out var minValue) &&
                    decimal.TryParse(maxValueObj.ToString(), out var maxValue))
                {
                    return currentValue >= minValue && currentValue <= maxValue;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a price above condition.
        /// </summary>
        /// <param name="price">The price threshold.</param>
        /// <returns>A new alert condition.</returns>
        public static AlertCondition PriceAbove(decimal price)
        {
            return new AlertCondition
            {
                Type = AlertType.Price,
                Operator = ComparisonOperator.GreaterThan,
                Threshold = price
            };
        }

        /// <summary>
        /// Creates a price below condition.
        /// </summary>
        /// <param name="price">The price threshold.</param>
        /// <returns>A new alert condition.</returns>
        public static AlertCondition PriceBelow(decimal price)
        {
            return new AlertCondition
            {
                Type = AlertType.Price,
                Operator = ComparisonOperator.LessThan,
                Threshold = price
            };
        }

        /// <summary>
        /// Creates a price between condition.
        /// </summary>
        /// <param name="minPrice">The minimum price.</param>
        /// <param name="maxPrice">The maximum price.</param>
        /// <returns>A new alert condition.</returns>
        public static AlertCondition PriceBetween(decimal minPrice, decimal maxPrice)
        {
            return new AlertCondition
            {
                Type = AlertType.Price,
                Operator = ComparisonOperator.Between,
                Threshold = minPrice,
                Parameters = new Dictionary<string, object>
                {
                    ["MinValue"] = minPrice,
                    ["MaxValue"] = maxPrice
                }
            };
        }

        /// <summary>
        /// Creates a percentage change condition.
        /// </summary>
        /// <param name="percentageChange">The percentage change threshold.</param>
        /// <param name="isPositive">Whether to trigger on positive or negative change.</param>
        /// <returns>A new alert condition.</returns>
        public static AlertCondition PercentageChange(decimal percentageChange, bool isPositive = true)
        {
            return new AlertCondition
            {
                Type = AlertType.PercentageChange,
                Operator = isPositive ? ComparisonOperator.GreaterThan : ComparisonOperator.LessThan,
                Threshold = percentageChange
            };
        }

        /// <summary>
        /// Creates a volume spike condition.
        /// </summary>
        /// <param name="volumeMultiplier">The volume multiplier threshold.</param>
        /// <returns>A new alert condition.</returns>
        public static AlertCondition VolumeSpike(decimal volumeMultiplier)
        {
            return new AlertCondition
            {
                Type = AlertType.Volume,
                Operator = ComparisonOperator.GreaterThan,
                Threshold = volumeMultiplier
            };
        }
    }




}

