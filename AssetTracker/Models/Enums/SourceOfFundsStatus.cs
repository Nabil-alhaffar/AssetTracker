namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the source of funds verification status of a user.
    /// </summary>
    public enum SourceOfFundsStatus
    {
        /// <summary>
        /// Source of funds verification not started.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Source of funds verification in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// Source of funds verified.
        /// </summary>
        Verified,

        /// <summary>
        /// Source of funds verification rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// Source of funds verification pending.
        /// </summary>
        Pending,

        /// <summary>
        /// Source of funds verification under review.
        /// </summary>
        UnderReview,

        /// <summary>
        /// Source of funds verification requires additional documentation.
        /// </summary>
        RequiresAdditionalDocs,

        /// <summary>
        /// Source of funds verification expired.
        /// </summary>
        Expired
    }
} 