using System;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services.Interfaces;
using AssetTracker.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AssetTracker.Models.DTOs;
using AssetTracker.Models.Enums;

namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves the current user's profile information.
        /// </summary>
        /// <returns>User profile information</returns>
        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            try
            {
                var userId = User.GetUserId();
                var user = await _userService.GetUserAsync(userId);
                
                if (user == null) 
                    return NotFound("User not found");

                // Return a focused profile response for the current user
                var profileResponse = new
                {
                    user.UserId,
                    user.FirstName,
                    user.LastName,
                    user.MiddleName,
                    user.FullName,
                    user.DisplayName,
                    user.Email,
                    user.SecondaryEmail,
                    user.UserName,
                    user.PhoneNumber,
                    user.MobileNumber,
                    user.DateOfBirth,
                    user.Age,
                    user.Gender,
                    ResidentialAddress = user.ResidentialAddress != null ? new
                    {
                        user.ResidentialAddress.StreetAddress1,
                        user.ResidentialAddress.StreetAddress2,
                        user.ResidentialAddress.City,
                        user.ResidentialAddress.State,
                        user.ResidentialAddress.PostalCode,
                        user.ResidentialAddress.Country
                    } : null,
                    user.CountryOfResidence,
                    user.Citizenship,
                    user.EmploymentStatus,
                    user.EmployerName,
                    user.JobTitle,
                    user.AnnualIncome,
                    user.NetWorth,
                    user.InvestmentExperience,
                    user.InvestmentObjectives,
                    user.RiskTolerance,
                    user.InvestmentTimeHorizon,
                    user.AccountType,
                    user.AccountStatus,
                    user.TradingPermissions,
                    user.MarginApprovalStatus,
                    user.OptionsApprovalStatus,
                    user.CryptoApprovalStatus,
                    user.KycStatus,
                    user.AmlStatus,
                    user.TimeZoneId,
                    user.PreferredLanguage,
                    user.PreferredCurrency,
                    user.TwoFactorEnabled,
                    user.IsLocked,
                    user.PasswordChangeRequired,
                    user.LastLoginDate,
                    user.LastActivityDate,
                    user.AccountOpenedDate,
                    user.CreatedAt,
                    user.UpdatedAt,
                    user.IsActive,
                    user.CanTrade
                };

                return Ok(profileResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving user profile: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a user by their unique identifier (admin only).
        /// Returns comprehensive user information including sensitive data.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <returns>Complete user information if found, otherwise 404 Not Found</returns>
        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserAsync(userId);
                if (user == null) 
                    return NotFound("User not found");

                // Return comprehensive user information for admin purposes
                var userResponse = MapUserToResponse(user);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving user: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a list of all registered users (admin only).
        /// </summary>
        /// <returns>List of users and the count, or 404 if none exist</returns>
        [HttpGet("Get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetUsersAsync();

                if (!users.Any())
                    return NotFound(new { message = "No users found." });

                var userResponses = users.Select(MapUserToResponse);
                return Ok(new { count = userResponses.Count(), users = userResponses });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving users: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the current user's profile information.
        /// </summary>
        /// <param name="request">Profile update information</param>
        /// <returns>Updated user profile</returns>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var user = await _userService.GetUserAsync(userId);
                
                if (user == null) 
                    return NotFound("User not found");

                // Update user fields if provided
                if (!string.IsNullOrEmpty(request.FirstName))
                    user.FirstName = request.FirstName;
                
                if (!string.IsNullOrEmpty(request.LastName))
                    user.LastName = request.LastName;
                
                if (!string.IsNullOrEmpty(request.MiddleName))
                    user.MiddleName = request.MiddleName;
                
                if (!string.IsNullOrEmpty(request.SecondaryEmail))
                    user.SecondaryEmail = request.SecondaryEmail;
                
                if (!string.IsNullOrEmpty(request.PhoneNumber))
                    user.PhoneNumber = request.PhoneNumber;
                
                if (!string.IsNullOrEmpty(request.MobileNumber))
                    user.MobileNumber = request.MobileNumber;
                
                if (!string.IsNullOrEmpty(request.TaxId))
                    user.TaxId = request.TaxId;

                // Update address information
                if (request.ResidentialAddress != null)
                {
                    user.ResidentialAddress = new Address
                    {
                        StreetAddress1 = request.ResidentialAddress.StreetAddress1,
                        StreetAddress2 = request.ResidentialAddress.StreetAddress2,
                        City = request.ResidentialAddress.City,
                        State = request.ResidentialAddress.State,
                        PostalCode = request.ResidentialAddress.PostalCode,
                        Country = request.ResidentialAddress.Country
                    };
                }

                if (request.MailingAddress != null)
                {
                    user.MailingAddress = new Address
                    {
                        StreetAddress1 = request.MailingAddress.StreetAddress1,
                        StreetAddress2 = request.MailingAddress.StreetAddress2,
                        City = request.MailingAddress.City,
                        State = request.MailingAddress.State,
                        PostalCode = request.MailingAddress.PostalCode,
                        Country = request.MailingAddress.Country
                    };
                }

                // Update employment & financial information
                if (request.EmploymentStatus.HasValue)
                    user.EmploymentStatus = request.EmploymentStatus.Value;
                
                if (!string.IsNullOrEmpty(request.EmployerName))
                    user.EmployerName = request.EmployerName;
                
                if (!string.IsNullOrEmpty(request.JobTitle))
                    user.JobTitle = request.JobTitle;
                
                if (request.AnnualIncome.HasValue)
                    user.AnnualIncome = request.AnnualIncome.Value;
                
                if (request.NetWorth.HasValue)
                    user.NetWorth = request.NetWorth.Value;
                
                if (request.LiquidNetWorth.HasValue)
                    user.LiquidNetWorth = request.LiquidNetWorth.Value;
                
                if (request.InvestmentExperience.HasValue)
                    user.InvestmentExperience = request.InvestmentExperience.Value;
                
                if (request.InvestmentObjectives != null)
                    user.InvestmentObjectives = request.InvestmentObjectives;
                
                if (request.RiskTolerance.HasValue)
                    user.RiskTolerance = request.RiskTolerance.Value;
                
                if (request.InvestmentTimeHorizon.HasValue)
                    user.InvestmentTimeHorizon = request.InvestmentTimeHorizon.Value;

                // Update preferences & settings
                if (!string.IsNullOrEmpty(request.TimeZoneId))
                    user.TimeZoneId = request.TimeZoneId;
                
                if (!string.IsNullOrEmpty(request.PreferredLanguage))
                    user.PreferredLanguage = request.PreferredLanguage;
                
                if (!string.IsNullOrEmpty(request.PreferredCurrency))
                    user.PreferredCurrency = request.PreferredCurrency;

                if (request.NotificationPreferences != null)
                {
                    user.NotificationPreferences = new NotificationPreferences
                    {
                        EmailNotifications = request.NotificationPreferences.EmailNotifications,
                        PushNotifications = request.NotificationPreferences.PushNotifications,
                        SmsNotifications = request.NotificationPreferences.SmsNotifications,
                        TradeConfirmations = request.NotificationPreferences.TradeConfirmations,
                        MarginCallAlerts = request.NotificationPreferences.MarginCallAlerts,
                        PriceAlerts = request.NotificationPreferences.PriceAlerts,
                        NewsAlerts = request.NotificationPreferences.NewsAlerts,
                        MarketingEmails = request.NotificationPreferences.MarketingEmails
                    };
                }

                if (request.TradingPreferences != null)
                {
                    user.TradingPreferences = new TradingPreferences
                    {
                        ConfirmTrades = request.TradingPreferences.ConfirmTrades,
                        ShowPnL = request.TradingPreferences.ShowPnL,
                        AutoSaveCharts = request.TradingPreferences.AutoSaveCharts,
                        DefaultOrderType = request.TradingPreferences.DefaultOrderType,
                        DefaultOrderDuration = request.TradingPreferences.DefaultOrderDuration
                    };
                }

                if (request.PrivacySettings != null)
                {
                    user.PrivacySettings = new PrivacySettings
                    {
                        SharePortfolioData = request.PrivacySettings.SharePortfolioData,
                        ShareTradingActivity = request.PrivacySettings.ShareTradingActivity,
                        AllowAnalytics = request.PrivacySettings.AllowAnalytics,
                        AllowMarketing = request.PrivacySettings.AllowMarketing
                    };
                }

                // Update security settings
                if (request.EnableTwoFactor.HasValue)
                    user.TwoFactorEnabled = request.EnableTwoFactor.Value;

                if (request.SecurityQuestions != null)
                {
                    user.SecurityQuestions = request.SecurityQuestions.Select(sq => new SecurityQuestion
                    {
                        Question = sq.Question,
                        AnswerHash = sq.Answer // Note: This should be hashed in production
                    }).ToList();
                }

                // Update system fields
                user.UpdatedAt = DateTime.UtcNow;
                user.AddAuditEvent(AuditEventType.ProfileUpdated, "User profile updated");

                await _userService.UpdateUserAsync(user);

                var userResponse = MapUserToResponse(user);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating profile: {ex.Message}");
            }
        }

        /// <summary>
        /// Submits KYC (Know Your Customer) information for verification.
        /// </summary>
        /// <param name="request">KYC information</param>
        /// <returns>KYC submission status</returns>
        [HttpPost("kyc")]
        public async Task<IActionResult> SubmitKyc([FromBody] KycRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var user = await _userService.GetUserAsync(userId);
                
                if (user == null) 
                    return NotFound("User not found");

                // Update user with KYC information
                user.DateOfBirth = request.DateOfBirth;
                user.Gender = request.Gender;
                user.ResidentialAddress = new Address
                {
                    StreetAddress1 = request.ResidentialAddress.StreetAddress1,
                    StreetAddress2 = request.ResidentialAddress.StreetAddress2,
                    City = request.ResidentialAddress.City,
                    State = request.ResidentialAddress.State,
                    PostalCode = request.ResidentialAddress.PostalCode,
                    Country = request.ResidentialAddress.Country
                };
                
                if (request.MailingAddress != null)
                {
                    user.MailingAddress = new Address
                    {
                        StreetAddress1 = request.MailingAddress.StreetAddress1,
                        StreetAddress2 = request.MailingAddress.StreetAddress2,
                        City = request.MailingAddress.City,
                        State = request.MailingAddress.State,
                        PostalCode = request.MailingAddress.PostalCode,
                        Country = request.MailingAddress.Country
                    };
                }

                user.CountryOfResidence = request.CountryOfResidence;
                user.Citizenship = request.Citizenship;
                user.TaxId = request.TaxId;
                user.EmploymentStatus = request.EmploymentStatus;
                user.EmployerName = request.EmployerName;
                user.JobTitle = request.JobTitle;
                user.AnnualIncome = request.AnnualIncome;
                user.NetWorth = request.NetWorth;
                user.LiquidNetWorth = request.LiquidNetWorth;
                user.InvestmentExperience = request.InvestmentExperience;
                user.InvestmentObjectives = request.InvestmentObjectives;
                user.RiskTolerance = request.RiskTolerance;
                user.InvestmentTimeHorizon = request.InvestmentTimeHorizon;
                user.IsPoliticallyExposedPerson = request.IsPoliticallyExposedPerson;

                // Update KYC status
                user.KycStatus = KycStatus.PendingReview;
                user.UpdatedAt = DateTime.UtcNow;
                user.AddAuditEvent(AuditEventType.KycStatusUpdated, "KYC information submitted for review");

                await _userService.UpdateUserAsync(user);

                return Ok(new { 
                    message = "KYC information submitted successfully",
                    kycStatus = user.KycStatus.ToString(),
                    nextSteps = "Your KYC information is under review. You will be notified once verified."
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error submitting KYC: {ex.Message}");
            }
        }

        /// <summary>
        /// Requests trading permissions for the current user.
        /// </summary>
        /// <param name="request">Trading permissions request</param>
        /// <returns>Trading permissions request status</returns>
        [HttpPost("trading-permissions")]
        public async Task<IActionResult> RequestTradingPermissions([FromBody] TradingPermissionsRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var user = await _userService.GetUserAsync(userId);
                
                if (user == null) 
                    return NotFound("User not found");

                // Update trading permissions based on request
                var newPermissions = new List<TradingPermission> { TradingPermission.Stocks }; // Basic permission

                if (request.EnableMarginTrading)
                {
                    user.MarginApprovalStatus = MarginApprovalStatus.Pending;
                    newPermissions.Add(TradingPermission.MarginTrading);
                }

                if (request.EnableOptionsTrading)
                {
                    user.OptionsApprovalStatus = request.RequestedOptionsLevel ?? OptionsApprovalStatus.Pending;
                    newPermissions.Add(TradingPermission.Options);
                }

                if (request.EnableCryptoTrading)
                {
                    user.CryptoApprovalStatus = CryptoApprovalStatus.Pending;
                    newPermissions.Add(TradingPermission.Cryptocurrencies);
                }

                if (request.EnableShortSelling)
                    newPermissions.Add(TradingPermission.ShortSelling);

                if (request.EnablePennyStockTrading)
                    newPermissions.Add(TradingPermission.PennyStocks);

                if (request.EnableAfterHoursTrading)
                    newPermissions.Add(TradingPermission.AfterHoursTrading);

                if (request.EnablePreMarketTrading)
                    newPermissions.Add(TradingPermission.PreMarketTrading);

                if (request.EnableInternationalTrading)
                    newPermissions.Add(TradingPermission.InternationalTrading);

                if (request.EnableLeveragedETFTrading)
                    newPermissions.Add(TradingPermission.LeveragedETFs);

                if (request.EnableInverseETFTrading)
                    newPermissions.Add(TradingPermission.InverseETFs);

                // Update trading limits
                if (request.RequestedMaxPositionSize.HasValue)
                    user.MaxPositionSize = request.RequestedMaxPositionSize.Value;

                if (request.RequestedDailyTradingLimit.HasValue)
                    user.DailyTradingLimit = request.RequestedDailyTradingLimit.Value;

                if (request.RequestedMaxLeverage.HasValue)
                    user.MaxLeverage = request.RequestedMaxLeverage.Value;

                user.TradingPermissions = newPermissions;
                user.UpdatedAt = DateTime.UtcNow;
                user.AddAuditEvent(AuditEventType.TradingPermissionsUpdated, "Trading permissions requested");

                await _userService.UpdateUserAsync(user);

                return Ok(new { 
                    message = "Trading permissions request submitted successfully",
                    requestedPermissions = newPermissions.Select(p => p.ToString()).ToList(),
                    nextSteps = "Your trading permissions request is under review. You will be notified once approved."
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error requesting trading permissions: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets the password for the specified user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="request">New password to be set</param>
        /// <returns>Confirmation message or error details</returns>
        [HttpPatch("{userId}/reset-password")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(Guid userId, [FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _userService.ResetPasswordAsync(userId, request.NewPassword);
                return Ok("Password has been successfully reset");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error resetting password: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets the email address for the specified user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="request">New email to be set</param>
        /// <returns>Confirmation message or error details</returns>
        [HttpPatch("{userId}/reset-email")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetEmail(Guid userId, [FromBody] ResetEmailRequest request)
        {
            try
            {
                await _userService.ResetEmailAsync(userId, request.NewEmail);
                return Ok("Email has been successfully reset");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error resetting email: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets the username for the specified user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="request">New username to be set</param>
        /// <returns>Confirmation message or error details</returns>
        [HttpPatch("{userId}/reset-username")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetUsername(Guid userId, [FromBody] ResetUsernameRequest request)
        {
            try
            {
                await _userService.ResetUsernameAsync(userId, request.NewUsername);
                return Ok("Username has been successfully reset");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error resetting username: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the time zone setting for the specified user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="model">Time zone information</param>
        /// <returns>Confirmation message on successful update</returns>
        [HttpPut("{userId}/timezone")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTimeZone(Guid userId, [FromBody] TimeZoneUpdateRequest model)
        {
            try
            {
                await _userService.UpdateUserTimeZoneAsync(userId, model.TimeZoneId);
                return Ok(new { message = "Time zone updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating timezone: {ex.Message}");
            }
        }

        #region Helper Methods

        private UserResponse MapUserToResponse(User user)
        {
            return new UserResponse
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                FullName = user.FullName,
                DisplayName = user.DisplayName,
                DateOfBirth = user.DateOfBirth,
                Age = user.Age,
                Gender = user.Gender,
                Email = user.Email,
                SecondaryEmail = user.SecondaryEmail,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                MobileNumber = user.MobileNumber,
                ResidentialAddress = user.ResidentialAddress != null ? new AddressResponse
                {
                    StreetAddress1 = user.ResidentialAddress.StreetAddress1,
                    StreetAddress2 = user.ResidentialAddress.StreetAddress2,
                    City = user.ResidentialAddress.City,
                    State = user.ResidentialAddress.State,
                    PostalCode = user.ResidentialAddress.PostalCode,
                    Country = user.ResidentialAddress.Country
                } : null,
                MailingAddress = user.MailingAddress != null ? new AddressResponse
                {
                    StreetAddress1 = user.MailingAddress.StreetAddress1,
                    StreetAddress2 = user.MailingAddress.StreetAddress2,
                    City = user.MailingAddress.City,
                    State = user.MailingAddress.State,
                    PostalCode = user.MailingAddress.PostalCode,
                    Country = user.MailingAddress.Country
                } : null,
                CountryOfResidence = user.CountryOfResidence,
                Citizenship = user.Citizenship,
                TaxId = user.TaxId,
                EmploymentStatus = user.EmploymentStatus,
                EmployerName = user.EmployerName,
                JobTitle = user.JobTitle,
                AnnualIncome = user.AnnualIncome,
                NetWorth = user.NetWorth,
                LiquidNetWorth = user.LiquidNetWorth,
                InvestmentExperience = user.InvestmentExperience,
                InvestmentObjectives = user.InvestmentObjectives,
                RiskTolerance = user.RiskTolerance,
                InvestmentTimeHorizon = user.InvestmentTimeHorizon,
                AccountType = user.AccountType,
                AccountStatus = user.AccountStatus,
                TradingPermissions = user.TradingPermissions,
                MarginApprovalStatus = user.MarginApprovalStatus,
                OptionsApprovalStatus = user.OptionsApprovalStatus,
                CryptoApprovalStatus = user.CryptoApprovalStatus,
                MaxPositionSize = user.MaxPositionSize,
                DailyTradingLimit = user.DailyTradingLimit,
                MaxLeverage = user.MaxLeverage,
                AccountOpenedDate = user.AccountOpenedDate,
                LastLoginDate = user.LastLoginDate,
                LastActivityDate = user.LastActivityDate,
                TwoFactorEnabled = user.TwoFactorEnabled,
                FailedLoginAttempts = user.FailedLoginAttempts,
                IsLocked = user.IsLocked,
                AccountLockoutEndTime = user.AccountLockoutEndTime,
                PasswordChangeRequired = user.PasswordChangeRequired,
                PasswordLastChanged = user.PasswordLastChanged,
                KycStatus = user.KycStatus,
                KycVerifiedDate = user.KycVerifiedDate,
                AmlStatus = user.AmlStatus,
                AmlScreenedDate = user.AmlScreenedDate,
                IsPoliticallyExposedPerson = user.IsPoliticallyExposedPerson,
                SourceOfFundsStatus = user.SourceOfFundsStatus,
                ComplianceNotesCount = user.ComplianceNotes.Count,
                RegulatoryRestrictionsCount = user.RegulatoryRestrictions.Count,
                TimeZoneId = user.TimeZoneId,
                PreferredLanguage = user.PreferredLanguage,
                PreferredCurrency = user.PreferredCurrency,
                NotificationPreferences = new NotificationPreferencesResponse
                {
                    EmailNotifications = user.NotificationPreferences.EmailNotifications,
                    PushNotifications = user.NotificationPreferences.PushNotifications,
                    SmsNotifications = user.NotificationPreferences.SmsNotifications,
                    TradeConfirmations = user.NotificationPreferences.TradeConfirmations,
                    MarginCallAlerts = user.NotificationPreferences.MarginCallAlerts,
                    PriceAlerts = user.NotificationPreferences.PriceAlerts,
                    NewsAlerts = user.NotificationPreferences.NewsAlerts,
                    MarketingEmails = user.NotificationPreferences.MarketingEmails
                },
                TradingPreferences = new TradingPreferencesResponse
                {
                    ConfirmTrades = user.TradingPreferences.ConfirmTrades,
                    ShowPnL = user.TradingPreferences.ShowPnL,
                    AutoSaveCharts = user.TradingPreferences.AutoSaveCharts,
                    DefaultOrderType = user.TradingPreferences.DefaultOrderType,
                    DefaultOrderDuration = user.TradingPreferences.DefaultOrderDuration
                },
                PrivacySettings = new PrivacySettingsResponse
                {
                    SharePortfolioData = user.PrivacySettings.SharePortfolioData,
                    ShareTradingActivity = user.PrivacySettings.ShareTradingActivity,
                    AllowAnalytics = user.PrivacySettings.AllowAnalytics,
                    AllowMarketing = user.PrivacySettings.AllowMarketing
                },
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Version = user.Version,
                IsMinor = user.IsMinor,
                IsActive = user.IsActive,
                CanTrade = user.CanTrade
            };
        }

        #endregion
    }
}

