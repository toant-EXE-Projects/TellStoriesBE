
namespace StoryTeller.Data.Utils
{
    public static class SafeCompare
    {
        public static string ToSafeCompare(this string inputStr)
        {
            if (string.IsNullOrWhiteSpace(inputStr))
                return string.Empty;

            string str = inputStr.Trim().ToLower();

            return str;
        }
    }
}
