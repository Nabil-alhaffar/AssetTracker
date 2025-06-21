namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the net worth range of a user.
    /// </summary>
    public enum NetWorthRange
    {
        /// <summary>
        /// Less than $10,000.
        /// </summary>
        LessThan10K,

        /// <summary>
        /// $10,000 - $24,999.
        /// </summary>
        TenToTwentyFiveK,

        /// <summary>
        /// $25,000 - $49,999.
        /// </summary>
        TwentyFiveToFiftyK,

        /// <summary>
        /// $50,000 - $99,999.
        /// </summary>
        FiftyToHundredK,

        /// <summary>
        /// $100,000 - $249,999.
        /// </summary>
        HundredToTwoHundredFiftyK,

        /// <summary>
        /// $250,000 - $499,999.
        /// </summary>
        TwoHundredFiftyToFiveHundredK,

        /// <summary>
        /// $500,000 - $999,999.
        /// </summary>
        FiveHundredToMillion,

        /// <summary>
        /// $1,000,000 - $2,499,999.
        /// </summary>
        MillionToTwoAndHalfMillion,

        /// <summary>
        /// $2,500,000 - $4,999,999.
        /// </summary>
        TwoAndHalfToFiveMillion,

        /// <summary>
        /// $5,000,000 or more.
        /// </summary>
        FiveMillionPlus,

        /// <summary>
        /// Prefer not to specify.
        /// </summary>
        PreferNotToSay
    }
} 