using System;
namespace AssetTracker.Services.Interfaces
{

    /// <summary>
    /// Interface for sending email notifications.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email to the specified recipient.
        /// </summary>
        /// <param name="to">The recipient email address.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="body">The email body.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendEmailAsync(string to, string subject, string body);
    }
}

