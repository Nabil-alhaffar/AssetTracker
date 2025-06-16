using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Defines types of financial transactions related to user accounts or portfolios.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        /// <summary>
        /// Represents a deposit transaction where funds are added.
        /// </summary>
        [EnumMember(Value = "DEPOSIT")]
        Deposit,

        /// <summary>
        /// Represents a withdrawal transaction where funds are removed.
        /// </summary>
        [EnumMember(Value = "WITHDRAWAL")]
        Withdrawal,

        /// <summary>
        /// Represents a transfer of funds between accounts.
        /// </summary>
        [EnumMember(Value = "TRANSFER")]
        Transfer,

        /// <summary>
        /// Represents a fee charged to the account.
        /// </summary>
        [EnumMember(Value = "FEE")]
        Fee,

        /// <summary>
        /// Represents interest earned or paid.
        /// </summary>
        [EnumMember(Value = "INTEREST")]
        Interest,

        /// <summary>
        /// Represents an adjustment transaction, such as corrections.
        /// </summary>
        [EnumMember(Value = "ADJUSTMENT")]
        Adjustment,
    }
}