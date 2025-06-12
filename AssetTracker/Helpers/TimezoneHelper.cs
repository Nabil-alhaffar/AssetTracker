using System;
namespace AssetTracker.Helpers
{
	public class TimezoneHelper
	{


        public static DateTime ConvertUtcToLocal(DateTime utcTime, string timeZoneId = "Pacific Standard Time")
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone);
        }
    }
}

