using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the cryptocurrency trading approval status of a user.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum CryptoApprovalStatus
    {
        /// <summary>
        /// Cryptocurrency trading not requested.
        /// </summary>
        NotRequested,

        /// <summary>
        /// Cryptocurrency trading request pending.
        /// </summary>
        Pending,

        /// <summary>
        /// Cryptocurrency trading approved.
        /// </summary>
        Approved,

        /// <summary>
        /// Cryptocurrency trading denied.
        /// </summary>
        Denied,

        /// <summary>
        /// Cryptocurrency trading suspended.
        /// </summary>
        Suspended,

        /// <summary>
        /// Cryptocurrency trading restricted.
        /// </summary>
        Restricted,

        /// <summary>
        /// Cryptocurrency trading under review.
        /// </summary>
        UnderReview,

        /// <summary>
        /// Cryptocurrency trading not available in user's jurisdiction.
        /// </summary>
        NotAvailable
    }
} 