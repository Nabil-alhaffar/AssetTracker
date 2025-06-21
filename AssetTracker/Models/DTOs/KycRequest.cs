using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the request payload for KYC (Know Your Customer) submission.
    /// </summary>
    public sealed record KycRequest
    {
        #region Personal Information

        /// <summary>
        /// User's date of birth.
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// User's gender.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// User's residential address.
        /// </summary>
        [Required]
        public AddressRequest ResidentialAddress { get; set; } = new();

        /// <summary>
        /// User's mailing address (if different from residential).
        /// </summary>
        public AddressRequest? MailingAddress { get; set; }

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
        [Required]
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
        [Required]
        public IncomeRange AnnualIncome { get; set; }

        /// <summary>
        /// User's net worth range.
        /// </summary>
        [Required]
        public NetWorthRange NetWorth { get; set; }

        /// <summary>
        /// User's liquid net worth range.
        /// </summary>
        [Required]
        public NetWorthRange LiquidNetWorth { get; set; }

        /// <summary>
        /// User's investment experience level.
        /// </summary>
        [Required]
        public InvestmentExperience InvestmentExperience { get; set; }

        /// <summary>
        /// User's investment objectives.
        /// </summary>
        [Required]
        public List<InvestmentObjective> InvestmentObjectives { get; set; } = new();

        /// <summary>
        /// User's risk tolerance level.
        /// </summary>
        [Required]
        public RiskTolerance RiskTolerance { get; set; }

        /// <summary>
        /// User's investment time horizon.
        /// </summary>
        [Required]
        public TimeHorizon InvestmentTimeHorizon { get; set; }

        #endregion

        #region Compliance & Legal

        /// <summary>
        /// Whether the user is a politically exposed person.
        /// </summary>
        [Required]
        public bool IsPoliticallyExposedPerson { get; set; }

        /// <summary>
        /// Whether the user has any regulatory restrictions.
        /// </summary>
        [Required]
        public bool HasRegulatoryRestrictions { get; set; }

        /// <summary>
        /// Whether the user has been convicted of a felony.
        /// </summary>
        [Required]
        public bool HasFelonyConviction { get; set; }

        /// <summary>
        /// Whether the user has been subject to regulatory action.
        /// </summary>
        [Required]
        public bool HasRegulatoryAction { get; set; }

        /// <summary>
        /// Source of funds description.
        /// </summary>
        [Required]
        [StringLength(500)]
        public string SourceOfFunds { get; set; } = string.Empty;

        /// <summary>
        /// Expected annual trading volume.
        /// </summary>
        [Required]
        public decimal ExpectedAnnualTradingVolume { get; set; }

        /// <summary>
        /// Expected maximum position size.
        /// </summary>
        [Required]
        public decimal ExpectedMaxPositionSize { get; set; }

        #endregion

        #region Documentation

        /// <summary>
        /// Government-issued ID document type.
        /// </summary>
        [Required]
        public string IdDocumentType { get; set; } = string.Empty;

        /// <summary>
        /// Government-issued ID document number.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string IdDocumentNumber { get; set; } = string.Empty;

        /// <summary>
        /// Government-issued ID document expiry date.
        /// </summary>
        [Required]
        public DateTime IdDocumentExpiryDate { get; set; }

        /// <summary>
        /// Government-issued ID document issuing country.
        /// </summary>
        [Required]
        [StringLength(3)]
        public string IdDocumentIssuingCountry { get; set; } = string.Empty;

        /// <summary>
        /// Proof of address document type.
        /// </summary>
        [Required]
        public string ProofOfAddressDocumentType { get; set; } = string.Empty;

        /// <summary>
        /// Proof of address document number.
        /// </summary>
        [StringLength(50)]
        public string? ProofOfAddressDocumentNumber { get; set; }

        /// <summary>
        /// Proof of address document issue date.
        /// </summary>
        [Required]
        public DateTime ProofOfAddressDocumentIssueDate { get; set; }

        #endregion

        #region Additional Information

        /// <summary>
        /// Additional comments or information.
        /// </summary>
        [StringLength(1000)]
        public string? AdditionalComments { get; set; }

        /// <summary>
        /// Whether the user agrees to electronic communications.
        /// </summary>
        [Required]
        public bool AgreeToElectronicCommunications { get; set; }

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

        #endregion
    }
} 