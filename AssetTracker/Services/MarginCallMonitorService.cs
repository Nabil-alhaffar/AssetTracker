using System;
using System.Threading;
using System.Threading.Tasks;
using AssetTracker.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AssetTracker.Services
{
    /// <summary>
    /// Background service that monitors for margin calls and sends automatic notifications.
    /// </summary>
    public class MarginCallMonitorService : BackgroundService
    {
        private readonly ILogger<MarginCallMonitorService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // Check every 5 minutes

        /// <summary>
        /// Initializes a new instance of the <see cref="MarginCallMonitorService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public MarginCallMonitorService(
            ILogger<MarginCallMonitorService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Executes the background service.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Margin Call Monitor Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckForMarginCallsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking for margin calls.");
                }

                // Wait for the next check interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Margin Call Monitor Service stopped.");
        }

        /// <summary>
        /// Checks for margin calls and sends notifications.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task CheckForMarginCallsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                _logger.LogDebug("Checking for margin calls...");

                // Get all users currently in margin call
                var marginCalls = await alertService.CheckMarginCallsAsync();

                if (marginCalls.Count > 0)
                {
                    _logger.LogWarning($"Found {marginCalls.Count} users in margin call.");

                    foreach (var marginCall in marginCalls)
                    {
                        try
                        {
                            // Send notification for each margin call
                            await notificationService.SendMarginCallNotificationAsync(marginCall);
                            
                            _logger.LogInformation($"Margin call notification sent for user {marginCall.UserId} " +
                                                  $"(Shortfall: ${marginCall.Shortfall:N2})");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to send margin call notification for user {marginCall.UserId}");
                        }
                    }
                }
                else
                {
                    _logger.LogDebug("No margin calls detected.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking for margin calls.");
            }
        }
    }
} 