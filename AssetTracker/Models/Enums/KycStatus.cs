using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the KYC (Know Your Customer) verification status of a user.
    /// </summary>
    ///
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum KycStatus
    {
        /// <summary>
        /// KYC not started.
        /// </summary>
        NotStarted,

        /// <summary>
        /// KYC in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// KYC pending review.
        /// </summary>
        PendingReview,

        /// <summary>
        /// KYC verified.
        /// </summary>
        Verified,

        /// <summary>
        /// KYC rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// KYC expired.
        /// </summary>
        Expired,

        /// <summary>
        /// KYC under review.
        /// </summary>
        UnderReview,

        /// <summary>
        /// KYC requires additional documentation.
        /// </summary>
        RequiresAdditionalDocs
    }
} 