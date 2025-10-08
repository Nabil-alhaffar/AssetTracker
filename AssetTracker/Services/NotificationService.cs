using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models.Alert;
using AssetTracker.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AssetTracker.Models.Enums;
namespace AssetTracker.Services
{
    /// <summary>
    /// Service for sending notifications to users.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly ISmsService _smsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="pushNotificationService">The push notification service.</param>
        /// <param name="smsService">The SMS service.</param>
        public NotificationService(
            ILogger<NotificationService> logger,
            IConfiguration configuration,
            IEmailService emailService,
            IPushNotificationService pushNotificationService,
            ISmsService smsService)
        {
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;
            _pushNotificationService = pushNotificationService;
            _smsService = smsService;
        }

        /// <summary>
        /// Sends a margin call notification to a user.
        /// </summary>
        /// <param name="marginCallAlert">The margin call alert details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendMarginCallNotificationAsync(MarginCallAlert marginCallAlert)
        {
            try
            {
                var subject = "🚨 MARGIN CALL ALERT - Action Required";
                var message = GenerateMarginCallMessage(marginCallAlert);

                // Send email notification
                if (!string.IsNullOrEmpty(marginCallAlert.UserEmail))
                {
                    await _emailService.SendEmailAsync(
                        marginCallAlert.UserEmail,
                        subject,
                        message
                    );
                    _logger.LogInformation($"Margin call email sent to {marginCallAlert.UserEmail}");
                }

                // Send push notification
                await _pushNotificationService.SendPushNotificationAsync(
                    marginCallAlert.UserId,
                    "Margin Call Alert",
                    $"Your account requires immediate attention. Shortfall: ${marginCallAlert.Shortfall:N2}"
                );

                // Send SMS notification (for critical alerts)
                if (marginCallAlert.Shortfall > 1000) // Only for significant shortfalls
                {
                    await _smsService.SendSmsAsync(
                        marginCallAlert.UserId,
                        $"MARGIN CALL: Your account needs ${marginCallAlert.Shortfall:N2} to resolve margin call."
                    );
                }

                _logger.LogInformation($"Margin call notifications sent for user {marginCallAlert.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send margin call notification for user {marginCallAlert.UserId}");
            }
        }

        /// <summary>
        /// Sends a stock alert notification to a user.
        /// </summary>
        /// <param name="alert">The stock alert that was triggered.</param>
        /// <param name="currentValue">The current value that triggered the alert.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendStockAlertNotificationAsync(StockAlert alert, decimal currentValue)
        {
            try
            {
                var subject = $"📈 Stock Alert: {alert.Symbol}";
                var message = GenerateStockAlertMessage(alert, currentValue);

                // Check notification preferences
                var prefs = alert.NotificationPreferences;

                // Send email if enabled
                if (prefs.EmailEnabled)
                {
                    // Get user email from alert or user service
                    var userEmail = await GetUserEmailAsync(alert.UserId);
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        await _emailService.SendEmailAsync(userEmail, subject, message);
                    }
                }

                // Send push notification if enabled
                if (prefs.PushEnabled)
                {
                    await _pushNotificationService.SendPushNotificationAsync(
                        alert.UserId,
                        $"Stock Alert: {alert.Symbol}",
                        $"{alert.Symbol} is now at ${currentValue:N2}"
                    );
                }

                // Send SMS if enabled
                if (prefs.SmsEnabled)
                {
                    await _smsService.SendSmsAsync(
                        alert.UserId,
                        $"Stock Alert: {alert.Symbol} at ${currentValue:N2}"
                    );
                }

                _logger.LogInformation($"Stock alert notification sent for {alert.Symbol} to user {alert.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send stock alert notification for alert {alert.AlertId}");
            }
        }

        /// <summary>
        /// Sends a portfolio summary notification to a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="summary">The portfolio summary.</param>
        /// <param name="frequency">The notification frequency.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendPortfolioSummaryNotificationAsync(Guid userId, object summary, NotificationFrequency frequency)
        {
            try
            {
                var subject = frequency switch
                {
                    NotificationFrequency.Daily => "📊 Daily Portfolio Summary",
                    NotificationFrequency.Weekly => "📈 Weekly Portfolio Summary",
                    _ => "📊 Portfolio Summary"
                };

                var message = GeneratePortfolioSummaryMessage(summary, frequency);

                var userEmail = await GetUserEmailAsync(userId);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    await _emailService.SendEmailAsync(userEmail, subject, message);
                }

                _logger.LogInformation($"Portfolio summary notification sent for user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send portfolio summary notification for user {userId}");
            }
        }

        /// <summary>
        /// Generates a margin call notification message.
        /// </summary>
        /// <param name="marginCallAlert">The margin call alert.</param>
        /// <returns>The formatted message.</returns>
        private string GenerateMarginCallMessage(MarginCallAlert marginCallAlert)
        {
            return $@"
🚨 MARGIN CALL ALERT 🚨

Dear {marginCallAlert.UserName},

Your trading account is currently in a MARGIN CALL and requires immediate attention.

📊 Account Status:
• Current Portfolio Value: ${marginCallAlert.PortfolioValue:N2}
• Margin Used: ${marginCallAlert.MarginUsed:N2}
• Margin Limit: ${marginCallAlert.MarginLimit:N2}
• Required Equity: ${marginCallAlert.RequiredEquity:N2}
• Shortfall Amount: ${marginCallAlert.Shortfall:N2}

⚠️ IMMEDIATE ACTION REQUIRED:
To resolve this margin call, you need to either:
1. Deposit ${marginCallAlert.Shortfall:N2} into your account
2. Close some positions to reduce margin usage
3. Contact support for assistance

⏰ Time is critical - please take action within 24 hours to avoid forced liquidation of positions.

🔗 Login to your account: [TradiumFinancial.com]

If you have any questions, please contact our support team immediately.

Best regards,
Your Tradium Financial Platform Team

---
This is an automated alert. Please do not reply to this email.
            ".Trim();
        }

        /// <summary>
        /// Generates a stock alert notification message.
        /// </summary>
        /// <param name="alert">The stock alert.</param>
        /// <param name="currentValue">The current value.</param>
        /// <returns>The formatted message.</returns>
        private string GenerateStockAlertMessage(StockAlert alert, decimal currentValue)
        {
            return $@"
📈 Stock Alert Triggered

Symbol: {alert.Symbol}
Current Price: ${currentValue:N2}
Alert Name: {alert.Name}
Condition: {alert.Condition.Type}
Threshold: ${alert.Condition.Threshold:N2}

Your alert has been triggered! The stock {alert.Symbol} has reached your specified condition.

🔗 View Details: [Your App URL]

---
This is an automated alert. Please do not reply to this email.
            ".Trim();
        }

        /// <summary>
        /// Generates a portfolio summary notification message.
        /// </summary>
        /// <param name="summary">The portfolio summary.</param>
        /// <param name="frequency">The notification frequency.</param>
        /// <returns>The formatted message.</returns>
        private string GeneratePortfolioSummaryMessage(object summary, NotificationFrequency frequency)
        {
            // This would be implemented based on your PortfolioSummary model
            return $@"
📊 {frequency} Portfolio Summary

Your portfolio summary is ready for review.

🔗 View Full Report: [Your App URL]

---
This is an automated notification. Please do not reply to this email.
            ".Trim();
        }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The user's email address.</returns>
        private async Task<string> GetUserEmailAsync(Guid userId)
        {
            // This would typically call your user service
            // For now, return a placeholder
            return "user@example.com";
        }
    }
} 