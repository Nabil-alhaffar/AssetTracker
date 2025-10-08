using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the type of trading account.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum AccountType
    {
        /// <summary>
        /// Individual account.
        /// </summary>
        Individual,

        /// <summary>
        /// Joint account.
        /// </summary>
        Joint,

        /// <summary>
        /// Corporate account.
        /// </summary>
        Corporate,

        /// <summary>
        /// Trust account.
        /// </summary>
        Trust,

        /// <summary>
        /// IRA account.
        /// </summary>
        IRA,

        /// <summary>
        /// Roth IRA account.
        /// </summary>
        RothIRA,

        /// <summary>
        /// 401(k) account.
        /// </summary>
        FourZeroOneK,

        /// <summary>
        /// 403(b) account.
        /// </summary>
        FourZeroThreeB,

        /// <summary>
        /// SEP IRA account.
        /// </summary>
        SepIRA,

        /// <summary>
        /// SIMPLE IRA account.
        /// </summary>
        SimpleIRA,

        /// <summary>
        /// 529 plan account.
        /// </summary>
        FiveTwoNine,

        /// <summary>
        /// HSA account.
        /// </summary>
        HSA,

        /// <summary>
        /// Custodial account.
        /// </summary>
        Custodial,

        /// <summary>
        /// Partnership account.
        /// </summary>
        Partnership,

        /// <summary>
        /// LLC account.
        /// </summary>
        LLC,

        /// <summary>
        /// Other account type.
        /// </summary>
        Other
    }
} 