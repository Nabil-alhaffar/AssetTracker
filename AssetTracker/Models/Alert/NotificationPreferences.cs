using System;
using AssetTracker.Models.Enums;
namespace AssetTracker.Models.Alert
{
    /// <summary>
    /// Represents notification preferences for an alert.
    /// </summary>
    public class NotificationPreferences
    {
        /// <summary>
        /// Gets or sets whether to send email notifications.
        /// </summary>
        public bool EmailEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to send push notifications.
        /// </summary>
        public bool PushEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets whether to send SMS notifications.
        /// </summary>
        public bool SmsEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets whether to send in-app notifications.
        /// </summary>
        public bool InAppEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the notification frequency (immediate, daily, weekly).
        /// </summary>
        public NotificationFrequency Frequency { get; set; } = NotificationFrequency.Immediate;
    }
}

