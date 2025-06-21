namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the options trading approval status of a user.
    /// </summary>
    public enum OptionsApprovalStatus
    {
        /// <summary>
        /// Options trading not requested.
        /// </summary>
        NotRequested,

        /// <summary>
        /// Options trading request pending.
        /// </summary>
        Pending,

        /// <summary>
        /// Level 1 options approval (covered calls and cash-secured puts).
        /// </summary>
        Level1,

        /// <summary>
        /// Level 2 options approval (long calls and puts).
        /// </summary>
        Level2,

        /// <summary>
        /// Level 3 options approval (spreads and straddles).
        /// </summary>
        Level3,

        /// <summary>
        /// Level 4 options approval (naked options).
        /// </summary>
        Level4,

        /// <summary>
        /// Level 5 options approval (unlimited).
        /// </summary>
        Level5,

        /// <summary>
        /// Options trading denied.
        /// </summary>
        Denied,

        /// <summary>
        /// Options trading suspended.
        /// </summary>
        Suspended,

        /// <summary>
        /// Options trading restricted.
        /// </summary>
        Restricted,

        /// <summary>
        /// Options trading under review.
        /// </summary>
        UnderReview
    }
} 