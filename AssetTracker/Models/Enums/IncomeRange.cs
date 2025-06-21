namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the annual income range of a user.
    /// </summary>
    public enum IncomeRange
    {
        /// <summary>
        /// Less than $25,000.
        /// </summary>
        LessThan25K,

        /// <summary>
        /// $25,000 - $49,999.
        /// </summary>
        TwentyFiveToFiftyK,

        /// <summary>
        /// $50,000 - $74,999.
        /// </summary>
        FiftyToSeventyFiveK,

        /// <summary>
        /// $75,000 - $99,999.
        /// </summary>
        SeventyFiveToHundredK,

        /// <summary>
        /// $100,000 - $149,999.
        /// </summary>
        HundredToHundredFiftyK,

        /// <summary>
        /// $150,000 - $199,999.
        /// </summary>
        HundredFiftyToTwoHundredK,

        /// <summary>
        /// $200,000 - $299,999.
        /// </summary>
        TwoHundredToThreeHundredK,

        /// <summary>
        /// $300,000 - $499,999.
        /// </summary>
        ThreeHundredToFiveHundredK,

        /// <summary>
        /// $500,000 - $999,999.
        /// </summary>
        FiveHundredToMillion,

        /// <summary>
        /// $1,000,000 or more.
        /// </summary>
        MillionPlus,

        /// <summary>
        /// Prefer not to specify.
        /// </summary>
        PreferNotToSay
    }
} 