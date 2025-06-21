using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AssetTracker.Models.Alert;
using AssetTracker.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AssetTracker.Services
{
    /// <summary>
    /// Background service that monitors stock alerts and sends automatic notifications.
    /// </summary>
    public class StockAlertMonitorService : BackgroundService
    {
        private readonly ILogger<StockAlertMonitorService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Check every minute

        /// <summary>
        /// Initializes a new instance of the <see cref="StockAlertMonitorService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public StockAlertMonitorService(
            ILogger<StockAlertMonitorService> logger,
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
            _logger.LogInformation("Stock Alert Monitor Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckStockAlertsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking stock alerts.");
                }

                // Wait for the next check interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Stock Alert Monitor Service stopped.");
        }

        /// <summary>
        /// Checks stock alerts and sends notifications for triggered alerts.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task CheckStockAlertsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                _logger.LogDebug("Checking stock alerts...");

                // Get all active alerts that need to be checked
                var alertsToCheck = await alertService.GetAlertsAsync();

                if (!alertsToCheck.Any())
                {
                    _logger.LogDebug("No active alerts to check.");
                    return;
                }

                _logger.LogDebug($"Checking {alertsToCheck.Count} active alerts.");

                // Group alerts by symbol to minimize API calls
                var alertsBySymbol = alertsToCheck.GroupBy(a => a.Symbol);

                foreach (var symbolGroup in alertsBySymbol)
                {
                    var symbol = symbolGroup.Key;
                    var alerts = symbolGroup.ToList();

                    try
                    {
                        // Get current price for this symbol (you might want to cache this)
                        var currentPrice = await GetCurrentStockPriceAsync(symbol);

                        // Check each alert for this symbol
                        foreach (var alert in alerts)
                        {
                            try
                            {
                                // Check if the alert should be triggered
                                var shouldTrigger = await alertService.CheckAlertAsync(alert);

                                if (shouldTrigger)
                                {
                                    _logger.LogInformation($"Alert triggered for {symbol} at ${currentPrice:N2} " +
                                                          $"(Alert: {alert.Name}, User: {alert.UserId})");

                                    // Mark the alert as triggered
                                    await alertService.MarkAlertAsTriggeredAsync(alert.AlertId);

                                    // Send notification
                                    await notificationService.SendStockAlertNotificationAsync(alert, currentPrice);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error checking alert {alert.AlertId} for symbol {symbol}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error getting price for symbol {symbol}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking stock alerts.");
            }
        }

        /// <summary>
        /// Gets the current stock price for a symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The current stock price.</returns>
        private async Task<decimal> GetCurrentStockPriceAsync(string symbol)
        {
            // This would typically call your stock market service
            // For now, return a mock price
            await Task.Delay(100); // Simulate API call delay
            return 150.00m; // Mock price
        }
    }
} 