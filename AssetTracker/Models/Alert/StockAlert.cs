using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models.Alert
{
    /// <summary>
    /// Represents a stock alert that can be triggered based on various conditions.
    /// </summary>
    public class StockAlert
    {
        /// <summary>
        /// Gets or sets the MongoDB ObjectId used as the primary key.
        /// </summary>
        [BsonId]
        public ObjectId MongoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the alert.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid AlertId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the unique identifier of the user who created the alert.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the stock symbol for this alert.
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the condition that triggers this alert.
        /// </summary>
        public AlertCondition Condition { get; set; } = new();

        /// <summary>
        /// Gets or sets whether the alert is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the alert has been triggered.
        /// </summary>
        public bool IsTriggered { get; set; } = false;

        /// <summary>
        /// Gets or sets when the alert was last triggered.
        /// </summary>
        public DateTime? LastTriggeredAt { get; set; }

        /// <summary>
        /// Gets or sets when the alert was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets when the alert was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the alert name/description.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the alert type (price, technical indicator, etc.).
        /// </summary>
        public AlertType Type { get; set; }

        /// <summary>
        /// Gets or sets the notification preferences for this alert.
        /// </summary>
        public NotificationPreferences NotificationPreferences { get; set; } = new();

        /// <summary>
        /// Checks if the alert should be triggered based on the current value.
        /// </summary>
        /// <param name="currentValue">The current value to check against.</param>
        /// <returns>True if the alert should be triggered; otherwise, false.</returns>
        public bool ShouldTrigger(decimal currentValue)
        {
            if (!IsActive || IsTriggered)
                return false;

            return Condition.IsTriggered(currentValue);
        }

        /// <summary>
        /// Marks the alert as triggered and updates the last triggered timestamp.
        /// </summary>
        public void TriggerAction()
        {
            IsTriggered = true;
            LastTriggeredAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Resets the alert so it can be triggered again.
        /// </summary>
        public void Reset()
        {
            IsTriggered = false;
            LastTriggeredAt = null;
            UpdatedAt = DateTime.UtcNow;
        }
    }




}

