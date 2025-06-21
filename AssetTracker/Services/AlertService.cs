using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using AssetTracker.Models;
using AssetTracker.Models.Alert;
using AssetTracker.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AssetTracker.Repositories.Interfaces;
using AssetTracker.Repositories.MongoDBRepositories;

namespace AssetTracker.Services
{
    /// <summary>
    /// Service for managing stock alerts and margin call monitoring.
    /// </summary>
    public class AlertService : IAlertService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string BaseUrl = "https://www.alphavantage.co/query";
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAlertRepository _alertRepository;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client for making API requests.</param>
        /// <param name="configuration">The configuration containing API keys.</param>
        /// <param name="portfolioRepository">The portfolio repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="alertRepository">The alert repository.</param>
        /// <param name="notificationService">The notification service.</param>
        public AlertService(
            HttpClient httpClient,
            IConfiguration configuration,
            IPortfolioRepository portfolioRepository,
            IUserRepository userRepository,
            IAlertRepository alertRepository,
            INotificationService notificationService)
        {
            _httpClient = httpClient;
            _apiKey = configuration["AlphaVantage:ApiKey"] ?? string.Empty;
            _portfolioRepository = portfolioRepository;
            _userRepository = userRepository;
            _alertRepository = alertRepository;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Adds a new stock alert to the system.
        /// </summary>
        /// <param name="alert">The stock alert to add.</param>
        public async Task AddAlertAsync(StockAlert alert)
        {
            await _alertRepository.AddAlertAsync(alert);
        }

        /// <summary>
        /// Removes a stock alert from the system.
        /// </summary>
        /// <param name="alertId">The ID of the alert to remove.</param>
        public async Task RemoveAlertAsync(Guid alertId)
        {
            await _alertRepository.DeleteAlertAsync(alertId);
        }

        /// <summary>
        /// Gets all active alerts.
        /// </summary>
        /// <returns>A list of all active alerts.</returns>
        public async Task<List<StockAlert>> GetAlertsAsync()
        {
            return await _alertRepository.GetAllActiveAlertsAsync();
        }

        /// <summary>
        /// Gets all alerts for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A list of alerts for the user.</returns>
        public async Task<List<StockAlert>> GetAlertsByUserIdAsync(Guid userId)
        {
            return await _alertRepository.GetAlertsByUserIdAsync(userId);
        }

        /// <summary>
        /// Gets active alerts for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A list of active alerts for the user.</returns>
        public async Task<List<StockAlert>> GetActiveAlertsByUserIdAsync(Guid userId)
        {
            return await _alertRepository.GetActiveAlertsByUserIdAsync(userId);
        }

        /// <summary>
        /// Updates an existing alert.
        /// </summary>
        /// <param name="alert">The alert to update.</param>
        public async Task UpdateAlertAsync(StockAlert alert)
        {
            await _alertRepository.UpdateAlertAsync(alert);
        }

        /// <summary>
        /// Checks if an alert should be triggered based on current market data.
        /// </summary>
        /// <param name="alert">The alert to check.</param>
        /// <returns>True if the alert should be triggered; otherwise, false.</returns>
        public async Task<bool> CheckAlertAsync(StockAlert alert)
        {
            try
            {
                var currentPrice = await GetStockPriceAsync(alert.Symbol);
                return alert.ShouldTrigger(currentPrice);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Marks an alert as triggered and sends notification.
        /// </summary>
        /// <param name="alertId">The alert ID.</param>
        public async Task MarkAlertAsTriggeredAsync(Guid alertId)
        {
            await _alertRepository.MarkAlertAsTriggeredAsync(alertId);
            
            // Get the alert details and send notification
            var alert = await _alertRepository.GetAlertByIdAsync(alertId);
            if (alert != null)
            {
                try
                {
                    var currentPrice = await GetStockPriceAsync(alert.Symbol);
                    await _notificationService.SendStockAlertNotificationAsync(alert, currentPrice);
                }
                catch (Exception ex)
                {
                    // Log the error but don't throw to prevent breaking the alert system
                    Console.WriteLine($"Failed to send notification for alert {alertId}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Resets a triggered alert.
        /// </summary>
        /// <param name="alertId">The alert ID.</param>
        public async Task ResetAlertAsync(Guid alertId)
        {
            await _alertRepository.ResetAlertAsync(alertId);
        }

        /// <summary>
        /// Gets triggered alerts for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="since">Optional date to filter alerts triggered since this time.</param>
        /// <returns>A list of triggered alerts.</returns>
        public async Task<List<StockAlert>> GetTriggeredAlertsByUserIdAsync(Guid userId, DateTime? since = null)
        {
            return await _alertRepository.GetTriggeredAlertsByUserIdAsync(userId, since);
        }

        /// <summary>
        /// Gets alert statistics for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>Alert statistics for the user.</returns>
        public async Task<AlertStatistics> GetAlertStatisticsByUserIdAsync(Guid userId)
        {
            return await _alertRepository.GetAlertStatisticsByUserIdAsync(userId);
        }

        /// <summary>
        /// Monitors all portfolios for margin calls and returns users in margin call.
        /// </summary>
        /// <returns>A list of users currently in margin call.</returns>
        public async Task<List<MarginCallAlert>> CheckMarginCallsAsync()
        {
            var marginCallAlerts = new List<MarginCallAlert>();
            
            try
            {
                var allUserIds = await _portfolioRepository.GetAllUserIdsAsync();
                
                foreach (var userId in allUserIds)
                {
                    var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
                    
                    if (portfolio.IsInMarginCall)
                    {
                        var user = await _userRepository.GetUserByIDAsync(userId);
                        var marginCallAlert = new MarginCallAlert
                        {
                            UserId = userId,
                            UserEmail = user?.Email ?? "Unknown",
                            UserName = user?.UserName ?? "Unknown",
                            PortfolioValue = portfolio.Equity,
                            MarginUsed = portfolio.MarginUsed,
                            MarginLimit = portfolio.MarginLimit,
                            RequiredEquity = portfolio.MarginUsed / (1 - portfolio.MaintenanceMarginRequirement),
                            Shortfall = (portfolio.MarginUsed / (1 - portfolio.MaintenanceMarginRequirement)) - portfolio.Equity,
                            TriggeredAt = DateTime.UtcNow
                        };
                        
                        marginCallAlerts.Add(marginCallAlert);
                        
                        // Send notification for margin call
                        try
                        {
                            await _notificationService.SendMarginCallNotificationAsync(marginCallAlert);
                        }
                        catch (Exception ex)
                        {
                            // Log the error but don't throw to prevent breaking the monitoring
                            Console.WriteLine($"Failed to send margin call notification for user {userId}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception but don't throw to prevent breaking the monitoring
                Console.WriteLine($"Error checking margin calls: {ex.Message}");
            }
            
            return marginCallAlerts;
        }

        /// <summary>
        /// Gets margin call status for a specific user.
        /// </summary>
        /// <param name="userId">The user ID to check.</param>
        /// <returns>Margin call alert if user is in margin call, otherwise null.</returns>
        public async Task<MarginCallAlert?> GetUserMarginCallStatusAsync(Guid userId)
        {
            try
            {
                var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
                
                if (portfolio.IsInMarginCall)
                {
                    var user = await _userRepository.GetUserByIDAsync(userId);
                    return new MarginCallAlert
                    {
                        UserId = userId,
                        UserEmail = user?.Email ?? "Unknown",
                        UserName = user?.UserName ?? "Unknown",
                        PortfolioValue = portfolio.Equity,
                        MarginUsed = portfolio.MarginUsed,
                        MarginLimit = portfolio.MarginLimit,
                        RequiredEquity = portfolio.MarginUsed / (1 - portfolio.MaintenanceMarginRequirement),
                        Shortfall = (portfolio.MarginUsed / (1 - portfolio.MaintenanceMarginRequirement)) - portfolio.Equity,
                        TriggeredAt = DateTime.UtcNow
                    };
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the current stock price for a symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The current stock price.</returns>
        private async Task<decimal> GetStockPriceAsync(string symbol)
        {
            var url = $"{BaseUrl}?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_apiKey}";
            var response = await _httpClient.GetStringAsync(url);
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
            
            if (data != null && data.ContainsKey("Global Quote"))
            {
                var globalQuote = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    data["Global Quote"].ToString() ?? "{}");
                
                if (globalQuote != null && globalQuote.ContainsKey("05. price"))
                {
                    if (decimal.TryParse(globalQuote["05. price"], out var price))
                    {
                        return price;
                    }
                }
            }
            
            throw new Exception($"Unable to get price for symbol {symbol}");
        }
    }
}

