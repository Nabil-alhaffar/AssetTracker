using System;
using System.Threading.Tasks;
using AssetTracker.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace AssetTracker.Services
{
    /// <summary>
    /// Mock implementation of the email service for development.
    /// </summary>
    public class MockEmailService : IEmailService
    {
        private readonly ILogger<MockEmailService> _logger;

        public MockEmailService(ILogger<MockEmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogInformation($"📧 MOCK EMAIL SENT:");
            _logger.LogInformation($"   To: {to}");
            _logger.LogInformation($"   Subject: {subject}");
            _logger.LogInformation($"   Body: {body}");
            
            await Task.Delay(100); // Simulate email sending delay
        }
    }

    /// <summary>
    /// Mock implementation of the push notification service for development.
    /// </summary>
    public class MockPushNotificationService : IPushNotificationService
    {
        private readonly ILogger<MockPushNotificationService> _logger;

        public MockPushNotificationService(ILogger<MockPushNotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendPushNotificationAsync(Guid userId, string title, string message)
        {
            _logger.LogInformation($"📱 MOCK PUSH NOTIFICATION SENT:");
            _logger.LogInformation($"   User: {userId}");
            _logger.LogInformation($"   Title: {title}");
            _logger.LogInformation($"   Message: {message}");
            
            await Task.Delay(50); // Simulate push notification delay
        }
    }

    /// <summary>
    /// Mock implementation of the SMS service for development.
    /// </summary>
    public class MockSmsService : ISmsService
    {
        private readonly ILogger<MockSmsService> _logger;

        public MockSmsService(ILogger<MockSmsService> logger)
        {
            _logger = logger;
        }

        public async Task SendSmsAsync(Guid userId, string message)
        {
            _logger.LogInformation($"📞 MOCK SMS SENT:");
            _logger.LogInformation($"   User: {userId}");
            _logger.LogInformation($"   Message: {message}");
            
            await Task.Delay(200); // Simulate SMS sending delay
        }
    }
} 