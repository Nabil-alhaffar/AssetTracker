using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AssetTracker.Models.Alert;
namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        /// <summary>
        /// Adds a new stock alert.
        /// </summary>
        /// <param name="alert">The stock alert to add</param>
        /// <returns>Success message</returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddAlert([FromBody] StockAlert alert)
        {
            try
            {
                await _alertService.AddAlertAsync(alert);
                return Ok(new { message = "Alert added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to add alert: {ex.Message}" });
            }
        }

        /// <summary>
        /// Removes a stock alert.
        /// </summary>
        /// <param name="alertId">The alert ID to remove</param>
        /// <returns>Success message</returns>
        [HttpDelete("remove/{alertId}")]
        public async Task<IActionResult> RemoveAlert(Guid alertId)
        {
            try
            {
                await _alertService.RemoveAlertAsync(alertId);
                return Ok(new { message = "Alert removed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to remove alert: {ex.Message}" });
            }
        }

        /// <summary>
        /// Gets all active alerts.
        /// </summary>
        /// <returns>List of all active alerts</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAlerts()
        {
            try
            {
                var alerts = await _alertService.GetAlertsAsync();
                return Ok(new { alerts });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get alerts: {ex.Message}" });
            }
        }

        /// <summary>
        /// Gets all alerts for a user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <returns>List of user's alerts</returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserAlerts(Guid userId)
        {
            try
            {
                var alerts = await _alertService.GetAlertsByUserIdAsync(userId);
                return Ok(new { alerts });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get user alerts: {ex.Message}" });
            }
        }

        /// <summary>
        /// Gets active alerts for a user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <returns>List of user's active alerts</returns>
        [HttpGet("user/{userId}/active")]
        public async Task<IActionResult> GetUserActiveAlerts(Guid userId)
        {
            try
            {
                var alerts = await _alertService.GetActiveAlertsByUserIdAsync(userId);
                return Ok(new { alerts });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get user active alerts: {ex.Message}" });
            }
        }

        /// <summary>
        /// Gets triggered alerts for a user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="since">Optional date to filter alerts triggered since this time</param>
        /// <returns>List of user's triggered alerts</returns>
        [HttpGet("user/{userId}/triggered")]
        public async Task<IActionResult> GetUserTriggeredAlerts(Guid userId, [FromQuery] DateTime? since = null)
        {
            try
            {
                var alerts = await _alertService.GetTriggeredAlertsByUserIdAsync(userId, since);
                return Ok(new { alerts });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get user triggered alerts: {ex.Message}" });
            }
        }

        /// <summary>
        /// Gets alert statistics for a user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <returns>Alert statistics for the user</returns>
        [HttpGet("user/{userId}/statistics")]
        public async Task<IActionResult> GetUserAlertStatistics(Guid userId)
        {
            try
            {
                var statistics = await _alertService.GetAlertStatisticsByUserIdAsync(userId);
                return Ok(new { statistics });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get user alert statistics: {ex.Message}" });
            }
        }

        /// <summary>
        /// Updates an existing alert.
        /// </summary>
        /// <param name="alert">The alert to update</param>
        /// <returns>Success message</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAlert([FromBody] StockAlert alert)
        {
            try
            {
                await _alertService.UpdateAlertAsync(alert);
                return Ok(new { message = "Alert updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to update alert: {ex.Message}" });
            }
        }

        /// <summary>
        /// Resets a triggered alert.
        /// </summary>
        /// <param name="alertId">The alert ID to reset</param>
        /// <returns>Success message</returns>
        [HttpPost("reset/{alertId}")]
        public async Task<IActionResult> ResetAlert(Guid alertId)
        {
            try
            {
                await _alertService.ResetAlertAsync(alertId);
                return Ok(new { message = "Alert reset successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to reset alert: {ex.Message}" });
            }
        }

        /// <summary>
        /// Checks for margin calls across all portfolios.
        /// </summary>
        /// <returns>List of users currently in margin call</returns>
        [HttpGet("margin-calls")]
        public async Task<IActionResult> CheckMarginCalls()
        {
            try
            {
                var marginCalls = await _alertService.CheckMarginCallsAsync();
                return Ok(new { marginCalls });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to check margin calls: {ex.Message}" });
            }
        }

        /// <summary>
        /// Gets margin call status for a specific user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <returns>Margin call status for the user</returns>
        [HttpGet("margin-calls/{userId}")]
        public async Task<IActionResult> GetUserMarginCallStatus(Guid userId)
        {
            try
            {
                var marginCallStatus = await _alertService.GetUserMarginCallStatusAsync(userId);
                return Ok(new { marginCallStatus });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get user margin call status: {ex.Message}" });
            }
        }

        /// <summary>
        /// Checks if a specific alert should be triggered.
        /// </summary>
        /// <param name="alertId">The alert ID to check</param>
        /// <returns>Whether the alert should be triggered</returns>
        [HttpPost("check/{alertId}")]
        public async Task<IActionResult> CheckAlert(Guid alertId)
        {
            try
            {
                // First get the alert
                var alerts = await _alertService.GetAlertsAsync();
                var alert = alerts.FirstOrDefault(a => a.AlertId == alertId);
                
                if (alert == null)
                {
                    return NotFound(new { message = "Alert not found." });
                }

                var shouldTrigger = await _alertService.CheckAlertAsync(alert);
                
                if (shouldTrigger)
                {
                    await _alertService.MarkAlertAsTriggeredAsync(alertId);
                }

                return Ok(new { shouldTrigger, alert });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to check alert: {ex.Message}" });
            }
        }
    }
}
