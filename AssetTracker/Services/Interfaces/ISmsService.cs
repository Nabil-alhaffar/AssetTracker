using System;
namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Interface for sending SMS notifications.
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// Sends an SMS to a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="message">The SMS message.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendSmsAsync(Guid userId, string message);
    }
}

