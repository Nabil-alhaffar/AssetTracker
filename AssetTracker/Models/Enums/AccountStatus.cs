using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the status of a user account.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountStatus
    {
        /// <summary>
        /// Account is pending approval.
        /// </summary>
        ///
        
        Pending,

        /// <summary>
        /// Account is active and can trade.
        /// </summary>
        Active,

        /// <summary>
        /// Account is suspended temporarily.
        /// </summary>
        Suspended,

        /// <summary>
        /// Account is restricted from certain activities.
        /// </summary>
        Restricted,

        /// <summary>
        /// Account is closed.
        /// </summary>
        Closed,

        /// <summary>
        /// Account is under review.
        /// </summary>
        UnderReview,

        /// <summary>
        /// Account is locked due to security concerns.
        /// </summary>
        Locked,

        /// <summary>
        /// Account is dormant due to inactivity.
        /// </summary>
        Dormant,

        /// <summary>
        /// Account is frozen due to regulatory requirements.
        /// </summary>
        Frozen
    }
} 