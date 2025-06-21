using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Models.Alert;
using AssetTracker.Repositories.Interfaces;

namespace AssetTracker.Repositories.MockRepositories
{
    /// <summary>
    /// Mock implementation of the <see cref="IAlertRepository"/> interface for testing.
    /// </summary>
    public class AlertRepository : IAlertRepository
    {
        private readonly Dictionary<Guid, StockAlert> _alerts = new();

        /// <summary>
        /// Adds a new alert to the mock storage.
        /// </summary>
        /// <param name="alert">The alert to add.</param>
        public async Task AddAlertAsync(StockAlert alert)
        {
            alert.CreatedAt = DateTime.UtcNow;
            alert.UpdatedAt = DateTime.UtcNow;
            _alerts[alert.AlertId] = alert;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves an alert by its unique identifier.
        /// </summary>
        /// <param name="alertId">The alert's unique identifier.</param>
        /// <returns>The alert if found; otherwise, null.</returns>
        public async Task<StockAlert?> GetAlertByIdAsync(Guid alertId)
        {
            _alerts.TryGetValue(alertId, out var alert);
            return await Task.FromResult(alert);
        }

        /// <summary>
        /// Retrieves all alerts for a specific user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A collection of alerts belonging to the user.</returns>
        public async Task<List<StockAlert>> GetAlertsByUserIdAsync(Guid userId)
        {
            var alerts = _alerts.Values
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
            return await Task.FromResult(alerts);
        }

        /// <summary>
        /// Retrieves all active alerts for a specific user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A collection of active alerts belonging to the user.</returns>
        public async Task<List<StockAlert>> GetActiveAlertsByUserIdAsync(Guid userId)
        {
            var alerts = _alerts.Values
                .Where(a => a.UserId == userId && a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
            return await Task.FromResult(alerts);
        }

        /// <summary>
        /// Retrieves all alerts for a specific symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A collection of alerts for the symbol.</returns>
        public async Task<List<StockAlert>> GetAlertsBySymbolAsync(string symbol)
        {
            var alerts = _alerts.Values
                .Where(a => a.Symbol == symbol && a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
            return await Task.FromResult(alerts);
        }

        /// <summary>
        /// Retrieves all active alerts.
        /// </summary>
        /// <returns>A collection of all active alerts.</returns>
        public async Task<List<StockAlert>> GetAllActiveAlertsAsync()
        {
            var alerts = _alerts.Values
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
            return await Task.FromResult(alerts);
        }

        /// <summary>
        /// Updates an existing alert.
        /// </summary>
        /// <param name="alert">The alert to update.</param>
        public async Task UpdateAlertAsync(StockAlert alert)
        {
            alert.UpdatedAt = DateTime.UtcNow;
            _alerts[alert.AlertId] = alert;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Deletes an alert by its unique identifier.
        /// </summary>
        /// <param name="alertId">The alert's unique identifier.</param>
        public async Task DeleteAlertAsync(Guid alertId)
        {
            _alerts.Remove(alertId);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Deletes all alerts for a specific user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        public async Task DeleteAlertsByUserIdAsync(Guid userId)
        {
            var alertsToRemove = _alerts.Values.Where(a => a.UserId == userId).ToList();
            foreach (var alert in alertsToRemove)
            {
                _alerts.Remove(alert.AlertId);
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves alerts that need to be checked (active and not recently triggered).
        /// </summary>
        /// <param name="symbol">Optional symbol to filter by.</param>
        /// <returns>A collection of alerts that need to be checked.</returns>
        public async Task<List<StockAlert>> GetAlertsToCheckAsync(string? symbol = null)
        {
            var alerts = _alerts.Values
                .Where(a => a.IsActive && !a.IsTriggered);

            if (!string.IsNullOrEmpty(symbol))
            {
                alerts = alerts.Where(a => a.Symbol == symbol);
            }

            var result = alerts.OrderByDescending(a => a.CreatedAt).ToList();
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Marks an alert as triggered.
        /// </summary>
        /// <param name="alertId">The alert's unique identifier.</param>
        public async Task MarkAlertAsTriggeredAsync(Guid alertId)
        {
            if (_alerts.TryGetValue(alertId, out var alert))
            {
                alert.IsTriggered = true;
                alert.LastTriggeredAt = DateTime.UtcNow;
                alert.UpdatedAt = DateTime.UtcNow;
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Resets a triggered alert so it can be triggered again.
        /// </summary>
        /// <param name="alertId">The alert's unique identifier.</param>
        public async Task ResetAlertAsync(Guid alertId)
        {
            if (_alerts.TryGetValue(alertId, out var alert))
            {
                alert.IsTriggered = false;
                alert.LastTriggeredAt = null;
                alert.UpdatedAt = DateTime.UtcNow;
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves triggered alerts for a specific user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="since">Optional date to filter alerts triggered since this time.</param>
        /// <returns>A collection of triggered alerts.</returns>
        public async Task<List<StockAlert>> GetTriggeredAlertsByUserIdAsync(Guid userId, DateTime? since = null)
        {
            var alerts = _alerts.Values
                .Where(a => a.UserId == userId && a.IsTriggered);

            if (since.HasValue)
            {
                alerts = alerts.Where(a => a.LastTriggeredAt >= since.Value);
            }

            var result = alerts.OrderByDescending(a => a.LastTriggeredAt).ToList();
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Retrieves alert statistics for a specific user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>Alert statistics including total, active, and triggered counts.</returns>
        public async Task<AlertStatistics> GetAlertStatisticsByUserIdAsync(Guid userId)
        {
            var userAlerts = _alerts.Values.Where(a => a.UserId == userId).ToList();
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);

            var statistics = new AlertStatistics
            {
                TotalAlerts = userAlerts.Count,
                ActiveAlerts = userAlerts.Count(a => a.IsActive),
                TriggeredAlerts = userAlerts.Count(a => a.IsTriggered),
                TriggeredToday = userAlerts.Count(a => a.IsTriggered && a.LastTriggeredAt >= today),
                TriggeredThisWeek = userAlerts.Count(a => a.IsTriggered && a.LastTriggeredAt >= weekStart)
            };

            return await Task.FromResult(statistics);
        }
    }
} 