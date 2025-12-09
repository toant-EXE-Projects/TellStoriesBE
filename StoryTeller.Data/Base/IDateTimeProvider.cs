namespace StoryTeller.Data.Base
{
    public interface IDateTimeProvider
    {
        DateTime GetUtcNow();
        DateTime GetCurrentTime(string timeZoneId);
        DateTime GetSystemCurrentTime();
        string FormatDateTime(DateTime dateTime);
    }
}
