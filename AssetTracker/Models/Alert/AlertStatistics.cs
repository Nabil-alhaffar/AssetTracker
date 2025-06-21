using System;
namespace AssetTracker.Models.Alert
{
    /// <summary>
    /// Represents statistics about alerts for a user.
    /// </summary>
    public class AlertStatistics
    {
        /// <summary>
        /// Gets or sets the total number of alerts.
        /// </summary>
        public int TotalAlerts { get; set; }

        /// <summary>
        /// Gets or sets the number of active alerts.
        /// </summary>
        public int ActiveAlerts { get; set; }

        /// <summary>
        /// Gets or sets the number of triggered alerts.
        /// </summary>
        public int TriggeredAlerts { get; set; }

        /// <summary>
        /// Gets or sets the number of alerts triggered today.
        /// </summary>
        public int TriggeredToday { get; set; }

        /// <summary>
        /// Gets or sets the number of alerts triggered this week.
        /// </summary>
        public int TriggeredThisWeek { get; set; }
    }
}

