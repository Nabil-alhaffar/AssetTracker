using System;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services.Interfaces;
using AssetTracker.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using AssetTracker.Models.DTOs;
using System.Collections.Generic;
using AssetTracker.Models.Enums;
namespace AssetTracker.Controllers
{
    /// <summary>
    /// Controller to handle user authentication, registration, login, logout, and token refresh.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IWatchlistService _watchlistService;
        private readonly IPortfolioService _portfolioService;
        private readonly IUserSessionManager _userSessionManager;
        private readonly SymbolSubscriptionManager _symbolSubscriptionManager;
        private readonly IPasswordService _passwordService;


        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="userService">User service for managing user data.</param>
        /// <param name="authService">Authentication service for JWT token generation.</param>
        /// <param name="symbolSubscriptionManager">Manager for real-time symbol subscriptions.</param>
        /// <param name="watchlistService">Service for managing user watchlists.</param>
        /// <param name="portfolioService">Service for managing user portfolios.</param>
        /// <param name="userSessionManager">Service for managing user sessions.</param>
        /// <param name="configuration">Configuration for accessing app settings.</param>
        /// <param name="passwordService">Service for password hashing and validation.</param>
        public AuthController(
            IUserService userService,
            IAuthService authService,
            SymbolSubscriptionManager symbolSubscriptionManager,
            IWatchlistService watchlistService,
            IPortfolioService portfolioService,
            IUserSessionManager userSessionManager,
            IConfiguration configuration,
            IPasswordService passwordService)
        {
            _userService = userService;
            _authService = authService;
            _watchlistService = watchlistService;
            _portfolioService = portfolioService;
            _symbolSubscriptionManager = symbolSubscriptionManager;
            _userSessionManager = userSessionManager;
            _configuration = configuration;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Registers a new user with the provided details.
        /// Automatically logs in the user after successful registration.
        /// </summary>
        /// <param name="model">Registration details including comprehensive user information.</param>
        /// <returns>Success message on registration and login, or error details.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Validate age requirement (must be 18 or older)
                var age = DateTime.UtcNow.Year - model.DateOfBirth.Year - 
                         (DateTime.UtcNow < model.DateOfBirth.AddYears(DateTime.UtcNow.Year - model.DateOfBirth.Year) ? 1 : 0);
                if (age < 18)
                {
                    return BadRequest(new { message = "User must be at least 18 years old to register." });
                }

                // Create a new user object with comprehensive information
                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Email = model.Email,
                    SecondaryEmail = model.SecondaryEmail,
                    PhoneNumber = model.PhoneNumber,
                    MobileNumber = model.MobileNumber,
                    
                    // Address information
                    ResidentialAddress = model.ResidentialAddress != null ? new Address
                    {
                        StreetAddress1 = model.ResidentialAddress.StreetAddress1,
                        StreetAddress2 = model.ResidentialAddress.StreetAddress2,
                        City = model.ResidentialAddress.City,
                        State = model.ResidentialAddress.State,
                        PostalCode = model.ResidentialAddress.PostalCode,
                        Country = model.ResidentialAddress.Country
                    } : null,
                    MailingAddress = model.MailingAddress != null ? new Address
                    {
                        StreetAddress1 = model.MailingAddress.StreetAddress1,
                        StreetAddress2 = model.MailingAddress.StreetAddress2,
                        City = model.MailingAddress.City,
                        State = model.MailingAddress.State,
                        PostalCode = model.MailingAddress.PostalCode,
                        Country = model.MailingAddress.Country
                    } : null,
                    CountryOfResidence = model.CountryOfResidence,
                    Citizenship = model.Citizenship,
                    TaxId = model.TaxId,

                    // Employment & Financial Information
                    EmploymentStatus = model.EmploymentStatus,
                    EmployerName = model.EmployerName,
                    JobTitle = model.JobTitle,
                    AnnualIncome = model.AnnualIncome,
                    NetWorth = model.NetWorth,
                    LiquidNetWorth = model.LiquidNetWorth,
                    InvestmentExperience = model.InvestmentExperience,
                    InvestmentObjectives = model.InvestmentObjectives,
                    RiskTolerance = model.RiskTolerance,
                    InvestmentTimeHorizon = model.InvestmentTimeHorizon,

                    // Account Information
                    AccountType = model.AccountType,
                    AccountStatus = AccountStatus.Pending, // Start with pending status
                    TradingPermissions = new List<TradingPermission> { TradingPermission.Stocks }, // Basic permission
                    MarginApprovalStatus = model.EnableMarginTrading ? MarginApprovalStatus.Pending : MarginApprovalStatus.NotRequested,
                    OptionsApprovalStatus = model.EnableOptionsTrading ? OptionsApprovalStatus.Pending : OptionsApprovalStatus.NotRequested,
                    CryptoApprovalStatus = model.EnableCryptoTrading ? CryptoApprovalStatus.Pending : CryptoApprovalStatus.NotRequested,

                    // Compliance & KYC/AML
                    KycStatus = KycStatus.NotStarted,
                    AmlStatus = AmlStatus.NotStarted,
                    IsPoliticallyExposedPerson = model.IsPoliticallyExposedPerson,
                    SourceOfFundsStatus = SourceOfFundsStatus.NotStarted,

                    // Preferences & Settings
                    TimeZoneId = model.TimeZoneId,
                    PreferredLanguage = model.PreferredLanguage,
                    PreferredCurrency = model.PreferredCurrency,
                    NotificationPreferences = new NotificationPreferences
                    {
                        EmailNotifications = model.NotificationPreferences.EmailNotifications,
                        PushNotifications = model.NotificationPreferences.PushNotifications,
                        SmsNotifications = model.NotificationPreferences.SmsNotifications,
                        TradeConfirmations = model.NotificationPreferences.TradeConfirmations,
                        MarginCallAlerts = model.NotificationPreferences.MarginCallAlerts,
                        PriceAlerts = model.NotificationPreferences.PriceAlerts,
                        NewsAlerts = model.NotificationPreferences.NewsAlerts,
                        MarketingEmails = model.NotificationPreferences.MarketingEmails
                    },
                    TradingPreferences = new TradingPreferences
                    {
                        ConfirmTrades = model.TradingPreferences.ConfirmTrades,
                        ShowPnL = model.TradingPreferences.ShowPnL,
                        AutoSaveCharts = model.TradingPreferences.AutoSaveCharts,
                        DefaultOrderType = model.TradingPreferences.DefaultOrderType,
                        DefaultOrderDuration = model.TradingPreferences.DefaultOrderDuration
                    },
                    PrivacySettings = new PrivacySettings
                    {
                        SharePortfolioData = model.PrivacySettings.SharePortfolioData,
                        ShareTradingActivity = model.PrivacySettings.ShareTradingActivity,
                        AllowAnalytics = model.PrivacySettings.AllowAnalytics,
                        AllowMarketing = model.PrivacySettings.AllowMarketing
                    },

                    // Security
                    TwoFactorEnabled = model.EnableTwoFactor,
                    SecurityQuestions = model.SecurityQuestions.Select(sq => new SecurityQuestion
                    {
                        Question = sq.Question,
                        AnswerHash = _passwordService.HashPassword(sq.Answer, _passwordService.GenerateSalt()) // Hash the answer
                    }).ToList(),

                    // System fields
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AccountOpenedDate = DateTime.UtcNow
                };

                // Add initial audit event
                user.AddAuditEvent(AuditEventType.AccountCreated, "User account created during registration");

                // Register the user and hash the password
                await _userService.RegisterUserAsync(user, model.Password);

                // Automatically log in the newly registered user
                await Login(new LoginRequest { UserName = model.UserName, Password = model.Password });

                return Ok(new { 
                    message = "User registered and logged in successfully",
                    userId = user.UserId,
                    accountStatus = user.AccountStatus.ToString(),
                    nextSteps = "Complete KYC verification to enable trading"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Logs out the currently authenticated user.
        /// Unsubscribes from all symbol feeds and clears refresh tokens.
        /// </summary>
        /// <param name="model">Logout request data including session ID.</param>
        /// <returns>Success message or error details.</returns>
        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest model)
        {
            try
            {
                // Get the bearer token from the Authorization header
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Missing token" });
                }

                var token = authHeader.Substring("Bearer ".Length);

                // Extract user ID from expired JWT token
                var principal = JwtHelpers.GetPrincipalFromExpiredToken(token, _configuration);
                var userId = principal.GetUserId();

                // Unsubscribe user from all real-time symbol feeds and end session
                await _symbolSubscriptionManager.UnsubscribeUserFromAllAsync(userId);
                await _userSessionManager.EndSessionAsync(userId, model.SessionId);

                // Delete refresh token cookie
                Response.Cookies.Delete("refreshToken");
                await _userService.ClearRefreshTokenAsync(userId);

                return Ok(new { message = "Logout successful (from expired token)" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Authenticates a user and issues JWT and refresh tokens.
        /// Subscribes the user to their watched and held symbols.
        /// </summary>
        /// <param name="request">Login credentials and optional timezone info.</param>
        /// <returns>Login response including tokens, session ID, and user details.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Validate username and password
                var user = await _userService.AuthenticateUserAsync(request.UserName, request.Password);

                // Generate access and refresh tokens
                var token = _authService.GenerateJwtToken(user);
                var refreshToken = _authService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                // Store refresh token in DB
                await _userService.UpdateUserRefreshTokenAsync(user.UserId, refreshToken, (DateTime)user.RefreshTokenExpiryTime);
                var sessionId = Guid.NewGuid().ToString();

                // Subscribe user to all symbols they watch or hold
                var watchlistsSymbols = await _watchlistService.GetAllWatchedTickersByUserIdAsync(user.UserId);
                var positionSymbols = (await _portfolioService.GetPortfolioPositionsAsync(user.UserId)).Keys.ToList();
                var combinedList = watchlistsSymbols.Concat(positionSymbols).Distinct();

                foreach (string symbol in combinedList)
                {
                    await _symbolSubscriptionManager.SubscribeUserToSymbolAsync(user.UserId, symbol);
                }

                // Update user's timezone if provided
                if (request.TimeZoneId != null && request.TimeZoneId != user.TimeZoneId)
                {
                    user.TimeZoneId = request.TimeZoneId;
                    await _userService.UpdateUserTimeZoneAsync(user.UserId, request.TimeZoneId);
                }

                // Register session for activity tracking
                await _userSessionManager.StartSessionAsync(
                    user.UserId,
                    sessionId,
                    Request.HttpContext.Connection.RemoteIpAddress?.ToString()!,
                    Request.Headers["User-Agent"]!
                );

                // Save refresh token in cookie
                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Send only over HTTPS
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    IsEssential = true,
                    Path = "/",
                    Domain = "ec2-18-188-45-142.us-east-2.compute.amazonaws.com"
                });

                // Return login info
                return Ok(new LoginResponse
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    SessionId = sessionId,
                    Token = token,
                    TimeZoneId = user.TimeZoneId,
                    Message = "Login Successful!"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Refreshes JWT access token using a valid refresh token.
        /// </summary>
        /// <returns>New JWT access token or an unauthorized response.</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                // Get bearer token from the header
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Missing or invalid authorization header" });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Extract principal and user ID from expired token
                var principal = JwtHelpers.GetPrincipalFromExpiredToken(token, _configuration);
                var userIdStr = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                    return Unauthorized();

                var user = await _userService.GetUserAsync(userId);
                var refreshToken = Request.Cookies["refreshToken"];

                // Validate refresh token
                if (string.IsNullOrEmpty(refreshToken) ||
                    user == null ||
                    user.RefreshToken != refreshToken ||
                    user.RefreshTokenExpiryTime < DateTime.UtcNow)
                {
                    return Unauthorized();
                }

                // Generate new tokens
                var newJwt = _authService.GenerateJwtToken(user);
                var newRefreshToken = _authService.GenerateRefreshToken();

                await _userService.UpdateUserRefreshTokenAsync(user.UserId, newRefreshToken, DateTime.UtcNow.AddDays(7));

                // Save new refresh token in cookie
                Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    IsEssential = true,
                    Path = "/",
                    Domain = "ec2-18-188-45-142.us-east-2.compute.amazonaws.com"
                });

                return Ok(new { token = newJwt });
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { message = "Invalid or expired token." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates an admin user (for initial setup or admin management).
        /// This endpoint should be secured in production.
        /// </summary>
        /// <param name="model">Admin registration details.</param>
        /// <returns>Success message on admin creation.</returns>
        [HttpPost("create-admin")]
        [AllowAnonymous] // Note: In production, this should be secured
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Validate age requirement (must be 18 or older)
                var age = DateTime.UtcNow.Year - model.DateOfBirth.Year - 
                         (DateTime.UtcNow < model.DateOfBirth.AddYears(DateTime.UtcNow.Year - model.DateOfBirth.Year) ? 1 : 0);
                if (age < 18)
                {
                    return BadRequest(new { message = "User must be at least 18 years old to register." });
                }

                // Create a new admin user object with comprehensive information
                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Email = model.Email,
                    SecondaryEmail = model.SecondaryEmail,
                    PhoneNumber = model.PhoneNumber,
                    MobileNumber = model.MobileNumber,
                    
                    // Address information
                    ResidentialAddress = model.ResidentialAddress != null ? new Address
                    {
                        StreetAddress1 = model.ResidentialAddress.StreetAddress1,
                        StreetAddress2 = model.ResidentialAddress.StreetAddress2,
                        City = model.ResidentialAddress.City,
                        State = model.ResidentialAddress.State,
                        PostalCode = model.ResidentialAddress.PostalCode,
                        Country = model.ResidentialAddress.Country
                    } : null,
                    MailingAddress = model.MailingAddress != null ? new Address
                    {
                        StreetAddress1 = model.MailingAddress.StreetAddress1,
                        StreetAddress2 = model.MailingAddress.StreetAddress2,
                        City = model.MailingAddress.City,
                        State = model.MailingAddress.State,
                        PostalCode = model.MailingAddress.PostalCode,
                        Country = model.MailingAddress.Country
                    } : null,
                    CountryOfResidence = model.CountryOfResidence,
                    Citizenship = model.Citizenship,
                    TaxId = model.TaxId,

                    // Employment & Financial Information
                    EmploymentStatus = model.EmploymentStatus,
                    EmployerName = model.EmployerName,
                    JobTitle = model.JobTitle,
                    AnnualIncome = model.AnnualIncome,
                    NetWorth = model.NetWorth,
                    LiquidNetWorth = model.LiquidNetWorth,
                    InvestmentExperience = model.InvestmentExperience,
                    InvestmentObjectives = model.InvestmentObjectives,
                    RiskTolerance = model.RiskTolerance,
                    InvestmentTimeHorizon = model.InvestmentTimeHorizon,

                    // Account Information - Admin gets full permissions
                    AccountType = model.AccountType,
                    AccountStatus = AccountStatus.Active, // Admin starts as active
                    TradingPermissions = new List<TradingPermission> 
                    { 
                        TradingPermission.Stocks,
                        TradingPermission.MarginTrading,
                        TradingPermission.Options,
                        TradingPermission.Cryptocurrencies,
                        TradingPermission.ShortSelling,
                        TradingPermission.PennyStocks,
                        TradingPermission.AfterHoursTrading,
                        TradingPermission.PreMarketTrading,
                        TradingPermission.InternationalTrading,
                        TradingPermission.LeveragedETFs,
                        TradingPermission.InverseETFs
                    },
                    MarginApprovalStatus = MarginApprovalStatus.Approved,
                    OptionsApprovalStatus = OptionsApprovalStatus.Level4,
                    CryptoApprovalStatus = CryptoApprovalStatus.Approved,

                    // Compliance & KYC/AML - Admin is pre-verified
                    KycStatus = KycStatus.Verified,
                    AmlStatus = AmlStatus.Cleared,
                    IsPoliticallyExposedPerson = model.IsPoliticallyExposedPerson,
                    SourceOfFundsStatus = SourceOfFundsStatus.Verified,

                    // Admin role
                    Roles = new List<string> { "Admin" },

                    // Preferences & Settings
                    TimeZoneId = model.TimeZoneId,
                    PreferredLanguage = model.PreferredLanguage,
                    PreferredCurrency = model.PreferredCurrency,
                    NotificationPreferences = new NotificationPreferences
                    {
                        EmailNotifications = model.NotificationPreferences.EmailNotifications,
                        PushNotifications = model.NotificationPreferences.PushNotifications,
                        SmsNotifications = model.NotificationPreferences.SmsNotifications,
                        TradeConfirmations = model.NotificationPreferences.TradeConfirmations,
                        MarginCallAlerts = model.NotificationPreferences.MarginCallAlerts,
                        PriceAlerts = model.NotificationPreferences.PriceAlerts,
                        NewsAlerts = model.NotificationPreferences.NewsAlerts,
                        MarketingEmails = model.NotificationPreferences.MarketingEmails
                    },
                    TradingPreferences = new TradingPreferences
                    {
                        ConfirmTrades = model.TradingPreferences.ConfirmTrades,
                        ShowPnL = model.TradingPreferences.ShowPnL,
                        AutoSaveCharts = model.TradingPreferences.AutoSaveCharts,
                        DefaultOrderType = model.TradingPreferences.DefaultOrderType,
                        DefaultOrderDuration = model.TradingPreferences.DefaultOrderDuration
                    },
                    PrivacySettings = new PrivacySettings
                    {
                        SharePortfolioData = model.PrivacySettings.SharePortfolioData,
                        ShareTradingActivity = model.PrivacySettings.ShareTradingActivity,
                        AllowAnalytics = model.PrivacySettings.AllowAnalytics,
                        AllowMarketing = model.PrivacySettings.AllowMarketing
                    },

                    // Security
                    TwoFactorEnabled = model.EnableTwoFactor,
                    SecurityQuestions = model.SecurityQuestions.Select(sq => new SecurityQuestion
                    {
                        Question = sq.Question,
                        AnswerHash = _passwordService.HashPassword(sq.Answer, _passwordService.GenerateSalt()) // Hash the answer
                    }).ToList(),

                    // System fields
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AccountOpenedDate = DateTime.UtcNow
                };

                // Add initial audit event
                user.AddAuditEvent(AuditEventType.AccountCreated, "Admin account created");

                // Register the admin user and hash the password
                await _userService.RegisterUserAsync(user, model.Password);

                return Ok(new { 
                    message = "Admin user created successfully",
                    userId = user.UserId,
                    username = user.UserName,
                    email = user.Email,
                    roles = user.Roles,
                    accountStatus = user.AccountStatus.ToString(),
                    note = "Admin user is ready to use with full permissions"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}