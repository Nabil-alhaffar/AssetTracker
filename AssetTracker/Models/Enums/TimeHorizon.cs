namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the investment time horizon of a user.
    /// </summary>
    public enum TimeHorizon
    {
        /// <summary>
        /// Less than 1 year.
        /// </summary>
        LessThanOneYear,

        /// <summary>
        /// 1-3 years.
        /// </summary>
        OneToThreeYears,

        /// <summary>
        /// 3-5 years.
        /// </summary>
        ThreeToFiveYears,

        /// <summary>
        /// 5-10 years.
        /// </summary>
        FiveToTenYears,

        /// <summary>
        /// 10-20 years.
        /// </summary>
        TenToTwentyYears,

        /// <summary>
        /// More than 20 years.
        /// </summary>
        MoreThanTwentyYears
    }
} 