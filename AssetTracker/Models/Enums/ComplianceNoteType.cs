namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the type of compliance note.
    /// </summary>
    public enum ComplianceNoteType
    {
        /// <summary>
        /// General compliance note.
        /// </summary>
        General,

        /// <summary>
        /// KYC compliance note.
        /// </summary>
        Kyc,

        /// <summary>
        /// AML compliance note.
        /// </summary>
        Aml,

        /// <summary>
        /// Source of funds compliance note.
        /// </summary>
        SourceOfFunds,

        /// <summary>
        /// Trading restriction note.
        /// </summary>
        TradingRestriction,

        /// <summary>
        /// Regulatory compliance note.
        /// </summary>
        Regulatory,

        /// <summary>
        /// Risk management note.
        /// </summary>
        RiskManagement,

        /// <summary>
        /// Account review note.
        /// </summary>
        AccountReview,

        /// <summary>
        /// Documentation note.
        /// </summary>
        Documentation,

        /// <summary>
        /// Warning note.
        /// </summary>
        Warning,

        /// <summary>
        /// Suspicious activity note.
        /// </summary>
        SuspiciousActivity,

        /// <summary>
        /// Other compliance note.
        /// </summary>
        Other
    }
} 