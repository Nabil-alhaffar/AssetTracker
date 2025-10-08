using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the risk tolerance level of a user.
    /// </summary>
    ///
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum RiskTolerance
    {
        /// <summary>
        /// Very conservative risk tolerance.
        /// </summary>
        VeryConservative,

        /// <summary>
        /// Conservative risk tolerance.
        /// </summary>
        Conservative,

        /// <summary>
        /// Moderate risk tolerance.
        /// </summary>
        Moderate,

        /// <summary>
        /// Aggressive risk tolerance.
        /// </summary>
        Aggressive,

        /// <summary>
        /// Very aggressive risk tolerance.
        /// </summary>
        VeryAggressive
    }
} 