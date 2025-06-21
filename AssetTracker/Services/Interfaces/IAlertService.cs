using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetTracker.Models.Alert;
namespace AssetTracker.Services.Interfaces
{
	/// <summary>
	/// Interface for managing stock alerts and margin call monitoring.
	/// </summary>
	public interface IAlertService
	{
		/// <summary>
		/// Adds a new stock alert to the system.
		/// </summary>
		/// <param name="alert">The stock alert to add.</param>
		Task AddAlertAsync(StockAlert alert);

		/// <summary>
		/// Removes a stock alert from the system.
		/// </summary>
		/// <param name="alertId">The ID of the alert to remove.</param>
		Task RemoveAlertAsync(Guid alertId);

		/// <summary>
		/// Gets all active alerts.
		/// </summary>
		/// <returns>A list of all active alerts.</returns>
		Task<List<StockAlert>> GetAlertsAsync();

		/// <summary>
		/// Gets all alerts for a specific user.
		/// </summary>
		/// <param name="userId">The user ID.</param>
		/// <returns>A list of alerts for the user.</returns>
		Task<List<StockAlert>> GetAlertsByUserIdAsync(Guid userId);

		/// <summary>
		/// Gets active alerts for a specific user.
		/// </summary>
		/// <param name="userId">The user ID.</param>
		/// <returns>A list of active alerts for the user.</returns>
		Task<List<StockAlert>> GetActiveAlertsByUserIdAsync(Guid userId);

		/// <summary>
		/// Updates an existing alert.
		/// </summary>
		/// <param name="alert">The alert to update.</param>
		Task UpdateAlertAsync(StockAlert alert);

		/// <summary>
		/// Checks if an alert should be triggered based on current market data.
		/// </summary>
		/// <param name="alert">The alert to check.</param>
		/// <returns>True if the alert should be triggered; otherwise, false.</returns>
		Task<bool> CheckAlertAsync(StockAlert alert);

		/// <summary>
		/// Marks an alert as triggered.
		/// </summary>
		/// <param name="alertId">The alert ID.</param>
		Task MarkAlertAsTriggeredAsync(Guid alertId);

		/// <summary>
		/// Resets a triggered alert.
		/// </summary>
		/// <param name="alertId">The alert ID.</param>
		Task ResetAlertAsync(Guid alertId);

		/// <summary>
		/// Gets triggered alerts for a specific user.
		/// </summary>
		/// <param name="userId">The user ID.</param>
		/// <param name="since">Optional date to filter alerts triggered since this time.</param>
		/// <returns>A list of triggered alerts.</returns>
		Task<List<StockAlert>> GetTriggeredAlertsByUserIdAsync(Guid userId, DateTime? since = null);

		/// <summary>
		/// Gets alert statistics for a specific user.
		/// </summary>
		/// <param name="userId">The user ID.</param>
		/// <returns>Alert statistics for the user.</returns>
		Task<AlertStatistics> GetAlertStatisticsByUserIdAsync(Guid userId);

		/// <summary>
		/// Monitors all portfolios for margin calls and returns users in margin call.
		/// </summary>
		/// <returns>A list of users currently in margin call.</returns>
		Task<List<MarginCallAlert>> CheckMarginCallsAsync();

		/// <summary>
		/// Gets margin call status for a specific user.
		/// </summary>
		/// <param name="userId">The user ID to check.</param>
		/// <returns>Margin call alert if user is in margin call, otherwise null.</returns>
		Task<MarginCallAlert?> GetUserMarginCallStatusAsync(Guid userId);
	}

	
	
}

