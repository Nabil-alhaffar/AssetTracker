using System;
namespace AssetTracker.Helpers
{

    /// <summary>
    /// Helper class for timezone-related operations.
    /// </summary>
	public class TimezoneHelper
	{

        /// <summary>
        /// Converts a UTC DateTime to a specified local timezone.
        /// Defaults to "Pacific Standard Time" if no timezone ID is provided.
        /// </summary>
        /// <param name="utcTime">The UTC time to convert.</param>
        /// <param name="timeZoneId">The target timezone ID (default is "Pacific Standard Time").</param>
        /// <returns>The converted DateTime in the target timezone.</returns>
        /// <exception cref="TimeZoneNotFoundException">Thrown when the timezone ID is invalid.</exception>
        /// <exception cref="InvalidTimeZoneException">Thrown when the timezone data is corrupt.</exception>
        public static DateTime ConvertUtcToLocal(DateTime utcTime, string timeZoneId = "Pacific Standard Time")
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone);
        }
    }
}

