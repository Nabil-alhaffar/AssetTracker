using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the request payload for updating user profile information.
    /// </summary>
    public sealed record UpdateUserProfileRequest
    {
        #region Personal Information

        /// <summary>
        /// The first name of the user.
        /// </summary>
        [StringLength(50)]
        public string? FirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        [StringLength(50)]
        public string? LastName { get; set; }

        /// <summary>
        /// The middle name or initial of the user.
        /// </summary>
        [StringLength(50)]
        public string? MiddleName { get; set; }

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
        /// The tax identification number of the user.
        /// </summary>
        [StringLength(50)]
        public string? TaxId { get; set; }

        #endregion

        #region Employment & Financial Information

        /// <summary>
        /// The employment status of the user.
        /// </summary>
        public EmploymentStatus? EmploymentStatus { get; set; }

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
        public IncomeRange? AnnualIncome { get; set; }

        /// <summary>
        /// The net worth range of the user.
        /// </summary>
        public NetWorthRange? NetWorth { get; set; }

        /// <summary>
        /// The liquid net worth range of the user.
        /// </summary>
        public NetWorthRange? LiquidNetWorth { get; set; }

        /// <summary>
        /// The investment experience level of the user.
        /// </summary>
        public InvestmentExperience? InvestmentExperience { get; set; }

        /// <summary>
        /// The investment objectives of the user.
        /// </summary>
        public List<InvestmentObjective>? InvestmentObjectives { get; set; }

        /// <summary>
        /// The risk tolerance level of the user.
        /// </summary>
        public RiskTolerance? RiskTolerance { get; set; }

        /// <summary>
        /// The investment time horizon of the user.
        /// </summary>
        public TimeHorizon? InvestmentTimeHorizon { get; set; }

        #endregion

        #region Preferences & Settings

        /// <summary>
        /// The time zone ID specified by the user.
        /// </summary>
        public string? TimeZoneId { get; set; }

        /// <summary>
        /// The preferred language of the user.
        /// </summary>
        public string? PreferredLanguage { get; set; }

        /// <summary>
        /// The preferred currency of the user.
        /// </summary>
        public string? PreferredCurrency { get; set; }

        /// <summary>
        /// The notification preferences of the user.
        /// </summary>
        public NotificationPreferencesRequest? NotificationPreferences { get; set; }

        /// <summary>
        /// The trading preferences of the user.
        /// </summary>
        public TradingPreferencesRequest? TradingPreferences { get; set; }

        /// <summary>
        /// The privacy settings of the user.
        /// </summary>
        public PrivacySettingsRequest? PrivacySettings { get; set; }

        #endregion

        #region Security

        /// <summary>
        /// Whether the user wants to enable two-factor authentication.
        /// </summary>
        public bool? EnableTwoFactor { get; set; }

        /// <summary>
        /// The security questions and answers for the user.
        /// </summary>
        public List<SecurityQuestionRequest>? SecurityQuestions { get; set; }

        #endregion
    }
} 