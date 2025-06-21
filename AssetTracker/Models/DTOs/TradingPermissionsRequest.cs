using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AssetTracker.Models.Enums;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the request payload for trading permissions.
    /// </summary>
    public sealed record TradingPermissionsRequest
    {
        #region Margin Trading

        /// <summary>
        /// Whether the user wants to enable margin trading.
        /// </summary>
        public bool EnableMarginTrading { get; set; }

        /// <summary>
        /// User's understanding of margin trading risks.
        /// </summary>
        [StringLength(1000)]
        public string? MarginTradingRiskUnderstanding { get; set; }

        /// <summary>
        /// User's experience with margin trading.
        /// </summary>
        [StringLength(1000)]
        public string? MarginTradingExperience { get; set; }

        /// <summary>
        /// Maximum leverage requested.
        /// </summary>
        public decimal? RequestedMaxLeverage { get; set; }

        #endregion

        #region Options Trading

        /// <summary>
        /// Whether the user wants to enable options trading.
        /// </summary>
        public bool EnableOptionsTrading { get; set; }

        /// <summary>
        /// User's understanding of options trading risks.
        /// </summary>
        [StringLength(1000)]
        public string? OptionsTradingRiskUnderstanding { get; set; }

        /// <summary>
        /// User's experience with options trading.
        /// </summary>
        [StringLength(1000)]
        public string? OptionsTradingExperience { get; set; }

        /// <summary>
        /// Maximum options level requested.
        /// </summary>
        public OptionsApprovalStatus? RequestedOptionsLevel { get; set; }

        #endregion

        #region Cryptocurrency Trading

        /// <summary>
        /// Whether the user wants to enable cryptocurrency trading.
        /// </summary>
        public bool EnableCryptoTrading { get; set; }

        /// <summary>
        /// User's understanding of cryptocurrency trading risks.
        /// </summary>
        [StringLength(1000)]
        public string? CryptoTradingRiskUnderstanding { get; set; }

        /// <summary>
        /// User's experience with cryptocurrency trading.
        /// </summary>
        [StringLength(1000)]
        public string? CryptoTradingExperience { get; set; }

        #endregion

        #region Other Trading Permissions

        /// <summary>
        /// Whether the user wants to enable short selling.
        /// </summary>
        public bool EnableShortSelling { get; set; }

        /// <summary>
        /// Whether the user wants to enable penny stock trading.
        /// </summary>
        public bool EnablePennyStockTrading { get; set; }

        /// <summary>
        /// Whether the user wants to enable after-hours trading.
        /// </summary>
        public bool EnableAfterHoursTrading { get; set; }

        /// <summary>
        /// Whether the user wants to enable pre-market trading.
        /// </summary>
        public bool EnablePreMarketTrading { get; set; }

        /// <summary>
        /// Whether the user wants to enable international trading.
        /// </summary>
        public bool EnableInternationalTrading { get; set; }

        /// <summary>
        /// Whether the user wants to enable leveraged ETF trading.
        /// </summary>
        public bool EnableLeveragedETFTrading { get; set; }

        /// <summary>
        /// Whether the user wants to enable inverse ETF trading.
        /// </summary>
        public bool EnableInverseETFTrading { get; set; }

        #endregion

        #region Trading Limits

        /// <summary>
        /// Requested maximum position size.
        /// </summary>
        public decimal? RequestedMaxPositionSize { get; set; }

        /// <summary>
        /// Requested daily trading limit.
        /// </summary>
        public decimal? RequestedDailyTradingLimit { get; set; }

        /// <summary>
        /// Requested maximum order size.
        /// </summary>
        public decimal? RequestedMaxOrderSize { get; set; }

        #endregion

        #region Risk Management

        /// <summary>
        /// User's understanding of trading risks.
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string TradingRiskUnderstanding { get; set; } = string.Empty;

        /// <summary>
        /// User's trading strategy description.
        /// </summary>
        [StringLength(1000)]
        public string? TradingStrategy { get; set; }

        /// <summary>
        /// User's risk management approach.
        /// </summary>
        [StringLength(1000)]
        public string? RiskManagementApproach { get; set; }

        /// <summary>
        /// User's stop-loss usage.
        /// </summary>
        public bool UsesStopLoss { get; set; }

        /// <summary>
        /// User's take-profit usage.
        /// </summary>
        public bool UsesTakeProfit { get; set; }

        #endregion

        #region Financial Information

        /// <summary>
        /// User's liquid assets available for trading.
        /// </summary>
        [Required]
        public decimal LiquidAssetsForTrading { get; set; }

        /// <summary>
        /// User's total investment portfolio value.
        /// </summary>
        [Required]
        public decimal TotalInvestmentPortfolioValue { get; set; }

        /// <summary>
        /// User's percentage of portfolio to be used for trading.
        /// </summary>
        [Required]
        [Range(0, 100)]
        public decimal PercentageOfPortfolioForTrading { get; set; }

        /// <summary>
        /// User's emergency fund amount.
        /// </summary>
        [Required]
        public decimal EmergencyFundAmount { get; set; }

        #endregion

        #region Compliance

        /// <summary>
        /// Whether the user understands the risks involved.
        /// </summary>
        [Required]
        public bool UnderstandsRisks { get; set; }

        /// <summary>
        /// Whether the user can afford to lose the money invested.
        /// </summary>
        [Required]
        public bool CanAffordToLose { get; set; }

        /// <summary>
        /// Whether the user has read and understood the risk disclosure.
        /// </summary>
        [Required]
        public bool HasReadRiskDisclosure { get; set; }

        /// <summary>
        /// Whether the user agrees to the terms and conditions.
        /// </summary>
        [Required]
        public bool AgreesToTerms { get; set; }

        /// <summary>
        /// Additional comments or information.
        /// </summary>
        [StringLength(1000)]
        public string? AdditionalComments { get; set; }

        #endregion
    }
} 