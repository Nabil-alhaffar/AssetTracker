using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the investment experience level of a user.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum InvestmentExperience
    {
        /// <summary>
        /// No investment experience.
        /// </summary>
        None,

        /// <summary>
        /// Limited investment experience (less than 1 year).
        /// </summary>
        Limited,

        /// <summary>
        /// Some investment experience (1-3 years).
        /// </summary>
        Some,

        /// <summary>
        /// Moderate investment experience (3-5 years).
        /// </summary>
        Moderate,

        /// <summary>
        /// Good investment experience (5-10 years).
        /// </summary>
        Good,

        /// <summary>
        /// Extensive investment experience (10+ years).
        /// </summary>
        Extensive,

        /// <summary>
        /// Professional investment experience.
        /// </summary>
        Professional
    }
} 