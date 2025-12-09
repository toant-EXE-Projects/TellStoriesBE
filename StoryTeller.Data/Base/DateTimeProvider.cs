using StoryTeller.Data.Constants;
using System.Globalization;

namespace StoryTeller.Data.Base
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetUtcNow() => DateTime.UtcNow;

        public DateTime GetCurrentTime(string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }

        public DateTime GetSystemCurrentTime()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeConstants.Vietnam);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }

        public string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString(TimeConstants.DateTimeFormat);
        }
    }
}
