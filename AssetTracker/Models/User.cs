using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents a user of the professional trading platform.
    /// </summary>
    public sealed record User
    {
        /// <summary>
        /// MongoDB internal identifier.
        /// </summary>
        [BsonId]
        public ObjectId MongoId { get; set; }

        /// <summary>
        /// Unique identifier for the user (GUID).
        /// </summary>
        [Required]
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        #region Personal Information

        /// <summary>
        /// User's first name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// User's middle name or initial.
        /// </summary>
        [StringLength(50)]
        public string? MiddleName { get; set; }

        /// <summary>
        /// User's date of birth for compliance and age verification.
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// User's gender for demographic purposes.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// User's email address (primary contact).
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Secondary email address for backup contact.
        /// </summary>
        [EmailAddress]
        [StringLength(100)]
        public string? SecondaryEmail { get; set; }

        /// <summary>
        /// Username for login (unique).
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number.
        /// </summary>
        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// User's mobile number for SMS notifications.
        /// </summary>
        [Phone]
        [StringLength(20)]
        public string? MobileNumber { get; set; }

        #endregion

        #region Address Information

        /// <summary>
        /// User's residential address.
        /// </summary>
        public Address? ResidentialAddress { get; set; }

        /// <summary>
        /// User's mailing address (if different from residential).
        /// </summary>
        public Address? MailingAddress { get; set; }

        /// <summary>
        /// User's country of residence.
        /// </summary>
        [Required]
        [StringLength(3)]
        public string CountryOfResidence { get; set; } = string.Empty;

        /// <summary>
        /// User's citizenship.
        /// </summary>
        [Required]
        [StringLength(3)]
        public string Citizenship { get; set; } = string.Empty;

        /// <summary>
        /// User's tax identification number.
        /// </summary>
        [StringLength(50)]
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
        [StringLength(100)]
        public string? EmployerName { get; set; }

        /// <summary>
        /// User's job title.
        /// </summary>
        [StringLength(100)]
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
        public DateTime AccountOpenedDate { get; set; } = DateTime.UtcNow;

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
        /// Hashed password for secure authentication.
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Salt used in hashing the password.
        /// </summary>
        [Required]
        public string PasswordSalt { get; set; } = string.Empty;

        /// <summary>
        /// Refresh token for maintaining authentication session.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Expiration time of the refresh token.
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; set; }

        /// <summary>
        /// Two-factor authentication enabled flag.
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Two-factor authentication secret key.
        /// </summary>
        public string? TwoFactorSecret { get; set; }

        /// <summary>
        /// User's security questions and answers.
        /// </summary>
        public List<SecurityQuestion> SecurityQuestions { get; set; } = new();

        /// <summary>
        /// User's failed login attempts count.
        /// </summary>
        public int FailedLoginAttempts { get; set; }

        /// <summary>
        /// User's account lockout end time.
        /// </summary>
        public DateTime? AccountLockoutEndTime { get; set; }

        /// <summary>
        /// User's password change required flag.
        /// </summary>
        public bool PasswordChangeRequired { get; set; }

        /// <summary>
        /// User's password last changed date.
        /// </summary>
        public DateTime? PasswordLastChanged { get; set; }

        /// <summary>
        /// User's roles for authorization.
        /// </summary>
        public List<string> Roles { get; set; } = new();

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
        /// User's politically exposed person (PEP) status.
        /// </summary>
        public bool IsPoliticallyExposedPerson { get; set; }

        /// <summary>
        /// User's source of funds verification status.
        /// </summary>
        public SourceOfFundsStatus SourceOfFundsStatus { get; set; }

        /// <summary>
        /// User's compliance notes and flags.
        /// </summary>
        public List<ComplianceNote> ComplianceNotes { get; set; } = new();

        /// <summary>
        /// User's regulatory restrictions.
        /// </summary>
        public List<RegulatoryRestriction> RegulatoryRestrictions { get; set; } = new();

        #endregion

        #region Preferences & Settings

        /// <summary>
        /// User's time zone identifier.
        /// </summary>
        public string TimeZoneId { get; set; } = "UTC";

        /// <summary>
        /// User's preferred language.
        /// </summary>
        public string PreferredLanguage { get; set; } = "en-US";

        /// <summary>
        /// User's preferred currency.
        /// </summary>
        public string PreferredCurrency { get; set; } = "USD";

        /// <summary>
        /// User's notification preferences.
        /// </summary>
        public NotificationPreferences NotificationPreferences { get; set; } = new();

        /// <summary>
        /// User's trading preferences.
        /// </summary>
        public TradingPreferences TradingPreferences { get; set; } = new();

        /// <summary>
        /// User's privacy settings.
        /// </summary>
        public PrivacySettings PrivacySettings { get; set; } = new();

        #endregion

        #region System & Audit Fields

        /// <summary>
        /// User's account creation date.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who created this account (for admin accounts).
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// User's last update date.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who last updated this account.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid? UpdatedBy { get; set; }

        /// <summary>
        /// User's account deletion date (soft delete).
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// User who deleted this account.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public Guid? DeletedBy { get; set; }

        /// <summary>
        /// User's account version for optimistic concurrency.
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// User's account audit trail.
        /// </summary>
        public List<AuditEvent> AuditTrail { get; set; } = new();

        #endregion

        #region Computed Properties

        /// <summary>
        /// User's full name.
        /// </summary>
        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// User's display name.
        /// </summary>
        [BsonIgnore]
        public string DisplayName => !string.IsNullOrEmpty(UserName) ? UserName : FullName;

        /// <summary>
        /// User's age.
        /// </summary>
        [BsonIgnore]
        public int Age => DateTime.UtcNow.Year - DateOfBirth.Year - (DateTime.UtcNow < DateOfBirth.AddYears(DateTime.UtcNow.Year - DateOfBirth.Year) ? 1 : 0);

        /// <summary>
        /// Whether the user is a minor.
        /// </summary>
        [BsonIgnore]
        public bool IsMinor => Age < 18;

        /// <summary>
        /// Whether the user's account is active.
        /// </summary>
        [BsonIgnore]
        public bool IsActive => AccountStatus == AccountStatus.Active && DeletedAt == null;

        /// <summary>
        /// Whether the user's account is locked.
        /// </summary>
        [BsonIgnore]
        public bool IsLocked => AccountLockoutEndTime.HasValue && AccountLockoutEndTime.Value > DateTime.UtcNow;

        /// <summary>
        /// Whether the user can trade.
        /// </summary>
        [BsonIgnore]
        public bool CanTrade => IsActive && !IsLocked && KycStatus == KycStatus.Verified && AmlStatus == AmlStatus.Cleared;

        #endregion

        #region Methods

        /// <summary>
        /// Increments the failed login attempts and locks the account if necessary.
        /// </summary>
        public void IncrementFailedLoginAttempts()
        {
            FailedLoginAttempts++;
            if (FailedLoginAttempts >= 5)
            {
                AccountLockoutEndTime = DateTime.UtcNow.AddMinutes(30);
            }
        }

        /// <summary>
        /// Resets the failed login attempts.
        /// </summary>
        public void ResetFailedLoginAttempts()
        {
            FailedLoginAttempts = 0;
            AccountLockoutEndTime = null;
        }

        /// <summary>
        /// Updates the last login information.
        /// </summary>
        public void UpdateLastLogin()
        {
            LastLoginDate = DateTime.UtcNow;
            LastActivityDate = DateTime.UtcNow;
            ResetFailedLoginAttempts();
        }

        /// <summary>
        /// Updates the last activity timestamp.
        /// </summary>
        public void UpdateLastActivity()
        {
            LastActivityDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds an audit event to the user's audit trail.
        /// </summary>
        /// <param name="eventType">The type of audit event.</param>
        /// <param name="description">The description of the event.</param>
        /// <param name="performedBy">The user who performed the action.</param>
        public void AddAuditEvent(AuditEventType eventType, string description, Guid? performedBy = null)
        {
            AuditTrail.Add(new AuditEvent
            {
                EventType = eventType,
                Description = description,
                PerformedBy = performedBy,
                Timestamp = DateTime.UtcNow
            });
        }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Represents a user's address.
    /// </summary>
    public class Address
    {
        public string StreetAddress1 { get; set; } = string.Empty;
        public string? StreetAddress2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a security question and answer.
    /// </summary>
    public class SecurityQuestion
    {
        public string Question { get; set; } = string.Empty;
        public string AnswerHash { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a compliance note.
    /// </summary>
    public class ComplianceNote
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Note { get; set; } = string.Empty;
        [BsonRepresentation(BsonType.String)]
        public Guid CreatedBy { get; set; }
        public ComplianceNoteType Type { get; set; }
    }

    /// <summary>
    /// Represents a regulatory restriction.
    /// </summary>
    public class RegulatoryRestriction
    {
        public string RestrictionType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string RegulatoryBody { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents notification preferences.
    /// </summary>
    public class NotificationPreferences
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
    /// Represents trading preferences.
    /// </summary>
    public class TradingPreferences
    {
        public bool ConfirmTrades { get; set; } = true;
        public bool ShowPnL { get; set; } = true;
        public bool AutoSaveCharts { get; set; } = false;
        public string DefaultOrderType { get; set; } = "Market";
        public int DefaultOrderDuration { get; set; } = 1; // days
    }

    /// <summary>
    /// Represents privacy settings.
    /// </summary>
    public class PrivacySettings
    {
        public bool SharePortfolioData { get; set; } = false;
        public bool ShareTradingActivity { get; set; } = false;
        public bool AllowAnalytics { get; set; } = true;
        public bool AllowMarketing { get; set; } = false;
    }

    /// <summary>
    /// Represents an audit event.
    /// </summary>
    public class AuditEvent
    {
        public AuditEventType EventType { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [BsonRepresentation(BsonType.String)]
        public Guid? PerformedBy { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    #endregion
} 