using System;
using System.Threading.Tasks;
using AssetTracker.Models.Alert;
using AssetTracker.Models.Enums;
namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Interface for sending notifications to users.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Sends a margin call notification to a user.
        /// </summary>
        /// <param name="marginCallAlert">The margin call alert details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendMarginCallNotificationAsync(MarginCallAlert marginCallAlert);

        /// <summary>
        /// Sends a stock alert notification to a user.
        /// </summary>
        /// <param name="alert">The stock alert that was triggered.</param>
        /// <param name="currentValue">The current value that triggered the alert.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendStockAlertNotificationAsync(StockAlert alert, decimal currentValue);

        /// <summary>
        /// Sends a portfolio summary notification to a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="summary">The portfolio summary.</param>
        /// <param name="frequency">The notification frequency.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendPortfolioSummaryNotificationAsync(Guid userId, object summary, NotificationFrequency frequency);
    }

} 