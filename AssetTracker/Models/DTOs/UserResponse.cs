using System;
using System.Collections.Generic;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the response payload for user information.
    /// </summary>
    public sealed record UserResponse
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        public Guid UserId { get; set; }

        #region Personal Information

        /// <summary>
        /// User's first name.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// User's middle name or initial.
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// User's full name.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// User's display name.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// User's date of birth.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// User's age.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// User's gender.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// User's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's secondary email address.
        /// </summary>
        public string? SecondaryEmail { get; set; }

        /// <summary>
        /// User's username.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// User's mobile number.
        /// </summary>
        public string? MobileNumber { get; set; }

        #endregion

        #region Address Information

        /// <summary>
        /// User's residential address.
        /// </summary>
        public AddressResponse? ResidentialAddress { get; set; }

        /// <summary>
        /// User's mailing address.
        /// </summary>
        public AddressResponse? MailingAddress { get; set; }

        /// <summary>
        /// User's country of residence.
        /// </summary>
        public string CountryOfResidence { get; set; } = string.Empty;

        /// <summary>
        /// User's citizenship.
        /// </summary>
        public string Citizenship { get; set; } = string.Empty;

        /// <summary>
        /// User's tax identification number.
        /// </summary>
        public string? TaxId { get; set; }

        #endregion

        #region Employment & Financial Information

        /// <summary>
        /// User's employment status.
        /// </summary>
        public EmploymentStatus EmploymentStatus { get; set; }

        /// <summary>
        /// User's employer name.
        /// </summary>
        public string? EmployerName { get; set; }

        /// <summary>
        /// User's job title.
        /// </summary>
        public string? JobTitle { get; set; }

        /// <summary>
        /// User's annual income range.
        /// </summary>
        public IncomeRange AnnualIncome { get; set; }

        /// <summary>
        /// User's net worth range.
        /// </summary>
        public NetWorthRange NetWorth { get; set; }

        /// <summary>
        /// User's liquid net worth range.
        /// </summary>
        public NetWorthRange LiquidNetWorth { get; set; }

        /// <summary>
        /// User's investment experience level.
        /// </summary>
        public InvestmentExperience InvestmentExperience { get; set; }

        /// <summary>
        /// User's investment objectives.
        /// </summary>
        public List<InvestmentObjective> InvestmentObjectives { get; set; } = new();

        /// <summary>
        /// User's risk tolerance level.
        /// </summary>
        public RiskTolerance RiskTolerance { get; set; }

        /// <summary>
        /// User's investment time horizon.
        /// </summary>
        public TimeHorizon InvestmentTimeHorizon { get; set; }

        #endregion

        #region Account & Trading Information

        /// <summary>
        /// User's account type.
        /// </summary>
        public AccountType AccountType { get; set; }

        /// <summary>
        /// User's account status.
        /// </summary>
        public AccountStatus AccountStatus { get; set; }

        /// <summary>
        /// User's trading permissions.
        /// </summary>
        public List<TradingPermission> TradingPermissions { get; set; } = new();

        /// <summary>
        /// User's margin trading approval status.
        /// </summary>
        public MarginApprovalStatus MarginApprovalStatus { get; set; }

        /// <summary>
        /// User's options trading approval status.
        /// </summary>
        public OptionsApprovalStatus OptionsApprovalStatus { get; set; }

        /// <summary>
        /// User's cryptocurrency trading approval status.
        /// </summary>
        public CryptoApprovalStatus CryptoApprovalStatus { get; set; }

        /// <summary>
        /// User's maximum position size limit.
        /// </summary>
        public decimal MaxPositionSize { get; set; }

        /// <summary>
        /// User's daily trading limit.
        /// </summary>
        public decimal DailyTradingLimit { get; set; }

        /// <summary>
        /// User's maximum leverage allowed.
        /// </summary>
        public decimal MaxLeverage { get; set; }

        /// <summary>
        /// User's account opening date.
        /// </summary>
        public DateTime AccountOpenedDate { get; set; }

        /// <summary>
        /// User's last login date.
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// User's last activity date.
        /// </summary>
        public DateTime? LastActivityDate { get; set; }

        #endregion

        #region Security & Authentication

        /// <summary>
        /// Whether two-factor authentication is enabled.
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// User's failed login attempts count.
        /// </summary>
        public int FailedLoginAttempts { get; set; }

        /// <summary>
        /// Whether the user's account is locked.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// User's account lockout end time.
        /// </summary>
        public DateTime? AccountLockoutEndTime { get; set; }

        /// <summary>
        /// Whether password change is required.
        /// </summary>
        public bool PasswordChangeRequired { get; set; }

        /// <summary>
        /// User's password last changed date.
        /// </summary>
        public DateTime? PasswordLastChanged { get; set; }

        #endregion

        #region Compliance & KYC/AML

        /// <summary>
        /// User's KYC verification status.
        /// </summary>
        public KycStatus KycStatus { get; set; }

        /// <summary>
        /// User's KYC verification date.
        /// </summary>
        public DateTime? KycVerifiedDate { get; set; }

        /// <summary>
        /// User's AML screening status.
        /// </summary>
        public AmlStatus AmlStatus { get; set; }

        /// <summary>
        /// User's AML screening date.
        /// </summary>
        public DateTime? AmlScreenedDate { get; set; }

        /// <summary>
        /// Whether the user is a politically exposed person.
        /// </summary>
        public bool IsPoliticallyExposedPerson { get; set; }

        /// <summary>
        /// User's source of funds verification status.
        /// </summary>
        public SourceOfFundsStatus SourceOfFundsStatus { get; set; }

        /// <summary>
        /// User's compliance notes count.
        /// </summary>
        public int ComplianceNotesCount { get; set; }

        /// <summary>
        /// User's regulatory restrictions count.
        /// </summary>
        public int RegulatoryRestrictionsCount { get; set; }

        #endregion

        #region Preferences & Settings

        /// <summary>
        /// User's time zone identifier.
        /// </summary>
        public string TimeZoneId { get; set; } = string.Empty;

        /// <summary>
        /// User's preferred language.
        /// </summary>
        public string PreferredLanguage { get; set; } = string.Empty;

        /// <summary>
        /// User's preferred currency.
        /// </summary>
        public string PreferredCurrency { get; set; } = string.Empty;

        /// <summary>
        /// User's notification preferences.
        /// </summary>
        public NotificationPreferencesResponse NotificationPreferences { get; set; } = new();

        /// <summary>
        /// User's trading preferences.
        /// </summary>
        public TradingPreferencesResponse TradingPreferences { get; set; } = new();

        /// <summary>
        /// User's privacy settings.
        /// </summary>
        public PrivacySettingsResponse PrivacySettings { get; set; } = new();

        #endregion

        #region System & Audit Fields

        /// <summary>
        /// User's account creation date.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// User's last update date.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// User's account version.
        /// </summary>
        public int Version { get; set; }

        #endregion

        #region Computed Properties

        /// <summary>
        /// Whether the user is a minor.
        /// </summary>
        public bool IsMinor { get; set; }

        /// <summary>
        /// Whether the user's account is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Whether the user can trade.
        /// </summary>
        public bool CanTrade { get; set; }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Represents an address response.
    /// </summary>
    public class AddressResponse
    {
        public string StreetAddress1 { get; set; } = string.Empty;
        public string? StreetAddress2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents notification preferences response.
    /// </summary>
    public class NotificationPreferencesResponse
    {
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public bool SmsNotifications { get; set; }
        public bool TradeConfirmations { get; set; }
        public bool MarginCallAlerts { get; set; }
        public bool PriceAlerts { get; set; }
        public bool NewsAlerts { get; set; }
        public bool MarketingEmails { get; set; }
    }

    /// <summary>
    /// Represents trading preferences response.
    /// </summary>
    public class TradingPreferencesResponse
    {
        public bool ConfirmTrades { get; set; }
        public bool ShowPnL { get; set; }
        public bool AutoSaveCharts { get; set; }
        public string DefaultOrderType { get; set; } = string.Empty;
        public int DefaultOrderDuration { get; set; }
    }

    /// <summary>
    /// Represents privacy settings response.
    /// </summary>
    public class PrivacySettingsResponse
    {
        public bool SharePortfolioData { get; set; }
        public bool ShareTradingActivity { get; set; }
        public bool AllowAnalytics { get; set; }
        public bool AllowMarketing { get; set; }
    }

    #endregion
} 