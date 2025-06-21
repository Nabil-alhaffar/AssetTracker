using System;
namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Interface for sending push notifications.
    /// </summary>
    public interface IPushNotificationService
    {
        /// <summary>
        /// Sends a push notification to a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="title">The notification title.</param>
        /// <param name="message">The notification message.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendPushNotificationAsync(Guid userId, string title, string message);
    }
}

