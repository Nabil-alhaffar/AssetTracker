namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the AML (Anti-Money Laundering) screening status of a user.
    /// </summary>
    public enum AmlStatus
    {
        /// <summary>
        /// AML screening not started.
        /// </summary>
        NotStarted,

        /// <summary>
        /// AML screening in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// AML screening cleared.
        /// </summary>
        Cleared,

        /// <summary>
        /// AML screening flagged.
        /// </summary>
        Flagged,

        /// <summary>
        /// AML screening blocked.
        /// </summary>
        Blocked,

        /// <summary>
        /// AML screening under review.
        /// </summary>
        UnderReview,

        /// <summary>
        /// AML screening requires manual review.
        /// </summary>
        RequiresManualReview,

        /// <summary>
        /// AML screening failed.
        /// </summary>
        Failed
    }
} 