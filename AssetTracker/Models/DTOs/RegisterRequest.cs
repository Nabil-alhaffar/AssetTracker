using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the request payload for user registration in a professional trading platform.
    /// </summary>
    public sealed record RegisterRequest
    {
        #region Basic Information

        /// <summary>
        /// The username chosen by the user.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// The first name of the user.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// The last name of the user.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// The middle name or initial of the user.
        /// </summary>
        [StringLength(50)]
        public string? MiddleName { get; set; }

        /// <summary>
        /// The date of birth of the user.
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// The gender of the user.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The secondary email address of the user.
        /// </summary>
        [EmailAddress]
        [StringLength(100)]
        public string? SecondaryEmail { get; set; }

        /// <summary>
        /// The phone number of the user.
        /// </summary>
        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// The mobile number of the user.
        /// </summary>
        [Phone]
        [StringLength(20)]
        public string? MobileNumber { get; set; }

        /// <summary>
        /// The password set by the user.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The confirmation password.
        /// </summary>
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        #endregion

        #region Address Information

        /// <summary>
        /// The residential address of the user.
        /// </summary>
        public AddressRequest? ResidentialAddress { get; set; }

        /// <summary>
        /// The mailing address of the user (if different from residential).
        /// </summary>
        public AddressRequest? MailingAddress { get; set; }

        /// <summary>
        /// The country of residence of the user.
        /// </summary>
        [Required]
        [StringLength(3)]
        public string CountryOfResidence { get; set; } = string.Empty;

        /// <summary>
        /// The citizenship of the user.
        /// </summary>
        [Required]
        [StringLength(3)]
        public string Citizenship { get; set; } = string.Empty;

        /// <summary>
        /// The tax identification number of the user.
        /// </summary>
        [StringLength(50)]
        public string? TaxId { get; set; }

        #endregion

        #region Employment & Financial Information

        /// <summary>
        /// The employment status of the user.
        /// </summary>
        public EmploymentStatus EmploymentStatus { get; set; }

        /// <summary>
        /// The employer name of the user.
        /// </summary>
        [StringLength(100)]
        public string? EmployerName { get; set; }

        /// <summary>
        /// The job title of the user.
        /// </summary>
        [StringLength(100)]
        public string? JobTitle { get; set; }

        /// <summary>
        /// The annual income range of the user.
        /// </summary>
        public IncomeRange AnnualIncome { get; set; }

        /// <summary>
        /// The net worth range of the user.
        /// </summary>
        public NetWorthRange NetWorth { get; set; }

        /// <summary>
        /// The liquid net worth range of the user.
        /// </summary>
        public NetWorthRange LiquidNetWorth { get; set; }

        /// <summary>
        /// The investment experience level of the user.
        /// </summary>
        public InvestmentExperience InvestmentExperience { get; set; }

        /// <summary>
        /// The investment objectives of the user.
        /// </summary>
        public List<InvestmentObjective> InvestmentObjectives { get; set; } = new();

        /// <summary>
        /// The risk tolerance level of the user.
        /// </summary>
        public RiskTolerance RiskTolerance { get; set; }

        /// <summary>
        /// The investment time horizon of the user.
        /// </summary>
        public TimeHorizon InvestmentTimeHorizon { get; set; }

        #endregion

        #region Account Information

        /// <summary>
        /// The type of account the user wants to open.
        /// </summary>
        public AccountType AccountType { get; set; }

        /// <summary>
        /// Whether the user wants to enable margin trading.
        /// </summary>
        public bool EnableMarginTrading { get; set; }

        /// <summary>
        /// Whether the user wants to enable options trading.
        /// </summary>
        public bool EnableOptionsTrading { get; set; }

        /// <summary>
        /// Whether the user wants to enable cryptocurrency trading.
        /// </summary>
        public bool EnableCryptoTrading { get; set; }

        #endregion

        #region Preferences & Settings

        /// <summary>
        /// The time zone ID specified by the user.
        /// </summary>
        public string TimeZoneId { get; set; } = "UTC";

        /// <summary>
        /// The preferred language of the user.
        /// </summary>
        public string PreferredLanguage { get; set; } = "en-US";

        /// <summary>
        /// The preferred currency of the user.
        /// </summary>
        public string PreferredCurrency { get; set; } = "USD";

        /// <summary>
        /// The notification preferences of the user.
        /// </summary>
        public NotificationPreferencesRequest NotificationPreferences { get; set; } = new();

        /// <summary>
        /// The trading preferences of the user.
        /// </summary>
        public TradingPreferencesRequest TradingPreferences { get; set; } = new();

        /// <summary>
        /// The privacy settings of the user.
        /// </summary>
        public PrivacySettingsRequest PrivacySettings { get; set; } = new();

        #endregion

        #region Compliance & Legal

        /// <summary>
        /// Whether the user agrees to the terms of service.
        /// </summary>
        [Required]
        public bool AgreeToTerms { get; set; }

        /// <summary>
        /// Whether the user agrees to the privacy policy.
        /// </summary>
        [Required]
        public bool AgreeToPrivacyPolicy { get; set; }

        /// <summary>
        /// Whether the user agrees to electronic communications.
        /// </summary>
        public bool AgreeToElectronicCommunications { get; set; }

        /// <summary>
        /// Whether the user is a politically exposed person.
        /// </summary>
        public bool IsPoliticallyExposedPerson { get; set; }

        /// <summary>
        /// Whether the user has any regulatory restrictions.
        /// </summary>
        public bool HasRegulatoryRestrictions { get; set; }

        /// <summary>
        /// Whether the user has been convicted of a felony.
        /// </summary>
        public bool HasFelonyConviction { get; set; }

        /// <summary>
        /// Whether the user has been subject to regulatory action.
        /// </summary>
        public bool HasRegulatoryAction { get; set; }

        #endregion

        #region Security

        /// <summary>
        /// The security questions and answers for the user.
        /// </summary>
        public List<SecurityQuestionRequest> SecurityQuestions { get; set; } = new();

        /// <summary>
        /// Whether the user wants to enable two-factor authentication.
        /// </summary>
        public bool EnableTwoFactor { get; set; }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Represents an address request.
    /// </summary>
    public class AddressRequest
    {
        [Required]
        [StringLength(100)]
        public string StreetAddress1 { get; set; } = string.Empty;

        [StringLength(100)]
        public string? StreetAddress2 { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [StringLength(3)]
        public string Country { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a security question request.
    /// </summary>
    public class SecurityQuestionRequest
    {
        [Required]
        [StringLength(200)]
        public string Question { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Answer { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents notification preferences request.
    /// </summary>
    public class NotificationPreferencesRequest
    {
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = false;
        public bool TradeConfirmations { get; set; } = true;
        public bool MarginCallAlerts { get; set; } = true;
        public bool PriceAlerts { get; set; } = true;
        public bool NewsAlerts { get; set; } = false;
        public bool MarketingEmails { get; set; } = false;
    }

    /// <summary>
    /// Represents trading preferences request.
    /// </summary>
    public class TradingPreferencesRequest
    {
        public bool ConfirmTrades { get; set; } = true;
        public bool ShowPnL { get; set; } = true;
        public bool AutoSaveCharts { get; set; } = false;
        public string DefaultOrderType { get; set; } = "Market";
        public int DefaultOrderDuration { get; set; } = 1; // days
    }

    /// <summary>
    /// Represents privacy settings request.
    /// </summary>
    public class PrivacySettingsRequest
    {
        public bool SharePortfolioData { get; set; } = false;
        public bool ShareTradingActivity { get; set; } = false;
        public bool AllowAnalytics { get; set; } = true;
        public bool AllowMarketing { get; set; } = false;
    }

    #endregion
}