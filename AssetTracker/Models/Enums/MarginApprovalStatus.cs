using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the margin trading approval status of a user.
    /// </summary>
    ///
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum MarginApprovalStatus
    {
        /// <summary>
        /// Margin trading not requested.
        /// </summary>
        NotRequested,

        /// <summary>
        /// Margin trading request pending.
        /// </summary>
        Pending,

        /// <summary>
        /// Margin trading approved.
        /// </summary>
        Approved,

        /// <summary>
        /// Margin trading denied.
        /// </summary>
        Denied,

        /// <summary>
        /// Margin trading suspended.
        /// </summary>
        Suspended,

        /// <summary>
        /// Margin trading restricted.
        /// </summary>
        Restricted,

        /// <summary>
        /// Margin trading under review.
        /// </summary>
        UnderReview
    }
} 