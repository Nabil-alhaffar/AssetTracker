using System;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services.Interfaces;
using AssetTracker.Helpers;
using Microsoft.AspNetCore.Authorization;
using AssetTracker.Models.Enums;
using AssetTracker.Models.DTOs;

namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ComplianceController : ControllerBase
    {
        private readonly IUserService _userService;

        public ComplianceController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets all users pending KYC review.
        /// </summary>
        /// <returns>List of users pending KYC review</returns>
        [HttpGet("kyc/pending")]
        public async Task<IActionResult> GetPendingKycUsers()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                var pendingKycUsers = users.Where(u => u.KycStatus == KycStatus.PendingReview);
                
                return Ok(new { 
                    count = pendingKycUsers.Count(),
                    users = pendingKycUsers.Select(u => new
                    {
                        u.UserId,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.KycStatus,
                        u.CreatedAt,
                        u.UpdatedAt
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving pending KYC users: {ex.Message}");
            }
        }

        /// <summary>
        /// Approves KYC for a user.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="notes">Approval notes</param>
        /// <returns>Approval confirmation</returns>
        [HttpPost("kyc/{userId}/approve")]
        public async Task<IActionResult> ApproveKyc(Guid userId, [FromBody] string? notes = null)
        {
            try
            {
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                user.KycStatus = KycStatus.Verified;
                user.KycVerifiedDate = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                user.AddAuditEvent(AuditEventType.KycStatusUpdated, $"KYC approved by admin. Notes: {notes ?? "No notes"}", User.GetUserId());

                await _userService.UpdateUserAsync(user);

                return Ok(new { 
                    message = "KYC approved successfully",
                    userId = user.UserId,
                    kycStatus = user.KycStatus.ToString(),
                    verifiedDate = user.KycVerifiedDate
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error approving KYC: {ex.Message}");
            }
        }

        /// <summary>
        /// Rejects KYC for a user.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="reason">Rejection reason</param>
        /// <returns>Rejection confirmation</returns>
        [HttpPost("kyc/{userId}/reject")]
        public async Task<IActionResult> RejectKyc(Guid userId, [FromBody] string reason)
        {
            try
            {
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                user.KycStatus = KycStatus.Rejected;
                user.UpdatedAt = DateTime.UtcNow;
                user.AddAuditEvent(AuditEventType.KycStatusUpdated, $"KYC rejected by admin. Reason: {reason}", User.GetUserId());

                // Add compliance note
                user.ComplianceNotes.Add(new ComplianceNote
                {
                    Timestamp = DateTime.UtcNow,
                    Note = $"KYC rejected: {reason}",
                    CreatedBy = User.GetUserId(),
                    Type = ComplianceNoteType.Kyc
                });

                await _userService.UpdateUserAsync(user);

                return Ok(new { 
                    message = "KYC rejected successfully",
                    userId = user.UserId,
                    kycStatus = user.KycStatus.ToString(),
                    reason = reason
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error rejecting KYC: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all users pending trading permissions review.
        /// </summary>
        /// <returns>List of users pending trading permissions review</returns>
        [HttpGet("trading-permissions/pending")]
        public async Task<IActionResult> GetPendingTradingPermissionsUsers()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                var pendingUsers = users.Where(u => 
                    u.MarginApprovalStatus == MarginApprovalStatus.Pending ||
                    u.OptionsApprovalStatus == OptionsApprovalStatus.Pending ||
                    u.CryptoApprovalStatus == CryptoApprovalStatus.Pending);
                
                return Ok(new { 
                    count = pendingUsers.Count(),
                    users = pendingUsers.Select(u => new
                    {
                        u.UserId,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        MarginApprovalStatus = u.MarginApprovalStatus.ToString(),
                        OptionsApprovalStatus = u.OptionsApprovalStatus.ToString(),
                        CryptoApprovalStatus = u.CryptoApprovalStatus.ToString(),
                        u.CreatedAt,
                        u.UpdatedAt
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving pending trading permissions users: {ex.Message}");
            }
        }

        /// <summary>
        /// Approves margin trading for a user.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="maxLeverage">Maximum leverage allowed</param>
        /// <returns>Approval confirmation</returns>
        [HttpPost("trading-permissions/{userId}/approve-margin")]
        public async Task<IActionResult> ApproveMarginTrading(Guid userId, [FromBody] decimal maxLeverage)
        {
            try
            {
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                user.MarginApprovalStatus = MarginApprovalStatus.Approved;
                user.MaxLeverage = maxLeverage;
                user.TradingPermissions.Add(TradingPermission.MarginTrading);
                user.UpdatedAt = DateTime.UtcNow;
                user.AddAuditEvent(AuditEventType.MarginApprovalUpdated, $"Margin trading approved with max leverage: {maxLeverage}", User.GetUserId());

                await _userService.UpdateUserAsync(user);

                return Ok(new { 
                    message = "Margin trading approved successfully",
                    userId = user.UserId,
                    marginApprovalStatus = user.MarginApprovalStatus.ToString(),
                    maxLeverage = user.MaxLeverage
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error approving margin trading: {ex.Message}");
            }
        }

        /// <summary>
        /// Approves options trading for a user.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="level">Options approval level</param>
        /// <returns>Approval confirmation</returns>
        [HttpPost("trading-permissions/{userId}/approve-options")]
        public async Task<IActionResult> ApproveOptionsTrading(Guid userId, [FromBody] OptionsApprovalStatus level)
        {
            try
            {
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                user.OptionsApprovalStatus = level;
                user.TradingPermissions.Add(TradingPermission.Options);
                user.UpdatedAt = DateTime.UtcNow;
                user.AddAuditEvent(AuditEventType.OptionsApprovalUpdated, $"Options trading approved at level: {level}", User.GetUserId());

                await _userService.UpdateUserAsync(user);

                return Ok(new { 
                    message = "Options trading approved successfully",
                    userId = user.UserId,
                    optionsApprovalStatus = user.OptionsApprovalStatus.ToString(),
                    level = level.ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error approving options trading: {ex.Message}");
            }
        }

        /// <summary>
        /// Approves cryptocurrency trading for a user.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Approval confirmation</returns>
        [HttpPost("trading-permissions/{userId}/approve-crypto")]
        public async Task<IActionResult> ApproveCryptoTrading(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                user.CryptoApprovalStatus = CryptoApprovalStatus.Approved;
                user.TradingPermissions.Add(TradingPermission.Cryptocurrencies);
                user.UpdatedAt = DateTime.UtcNow;
                user.AddAuditEvent(AuditEventType.CryptoApprovalUpdated, "Cryptocurrency trading approved", User.GetUserId());

                await _userService.UpdateUserAsync(user);

                return Ok(new { 
                    message = "Cryptocurrency trading approved successfully",
                    userId = user.UserId,
                    cryptoApprovalStatus = user.CryptoApprovalStatus.ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error approving cryptocurrency trading: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a compliance note to a user.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="note">Compliance note</param>
        /// <param name="type">Type of compliance note</param>
        /// <returns>Note addition confirmation</returns>
        [HttpPost("{userId}/compliance-notes")]
        public async Task<IActionResult> AddComplianceNote(Guid userId, [FromBody] ComplianceNoteRequest request)
        {
            try
            {
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                user.ComplianceNotes.Add(new ComplianceNote
                {
                    Timestamp = DateTime.UtcNow,
                    Note = request.Note,
                    CreatedBy = User.GetUserId(),
                    Type = request.Type
                });

                user.UpdatedAt = DateTime.UtcNow;
                user.AddAuditEvent(AuditEventType.ComplianceNoteAdded, $"Compliance note added: {request.Note}", User.GetUserId());

                await _userService.UpdateUserAsync(user);

                return Ok(new { 
                    message = "Compliance note added successfully",
                    userId = user.UserId,
                    noteCount = user.ComplianceNotes.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding compliance note: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets compliance notes for a user.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of compliance notes</returns>
        [HttpGet("{userId}/compliance-notes")]
        public async Task<IActionResult> GetComplianceNotes(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                return Ok(new { 
                    userId = user.UserId,
                    notes = user.ComplianceNotes.OrderByDescending(n => n.Timestamp)
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving compliance notes: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates user account status.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="status">New account status</param>
        /// <param name="reason">Reason for status change</param>
        /// <returns>Status update confirmation</returns>
        [HttpPut("{userId}/account-status")]
        public async Task<IActionResult> UpdateAccountStatus(Guid userId, [FromBody] AccountStatusUpdateRequest request)
        {
            try
            {
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                var previousStatus = user.AccountStatus;
                user.AccountStatus = request.Status;
                user.UpdatedAt = DateTime.UtcNow;
                user.AddAuditEvent(AuditEventType.AccountStatusChanged, $"Account status changed from {previousStatus} to {request.Status}. Reason: {request.Reason}", User.GetUserId());

                await _userService.UpdateUserAsync(user);

                return Ok(new { 
                    message = "Account status updated successfully",
                    userId = user.UserId,
                    previousStatus = previousStatus.ToString(),
                    newStatus = user.AccountStatus.ToString(),
                    reason = request.Reason
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating account status: {ex.Message}");
            }
        }
    }

    #region Supporting Classes

    /// <summary>
    /// Represents a compliance note request.
    /// </summary>
    public class ComplianceNoteRequest
    {
        public string Note { get; set; } = string.Empty;
        public ComplianceNoteType Type { get; set; }
    }

    /// <summary>
    /// Represents an account status update request.
    /// </summary>
    public class AccountStatusUpdateRequest
    {
        public AccountStatus Status { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    #endregion
} 