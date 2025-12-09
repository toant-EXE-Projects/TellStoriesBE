using System.Text.RegularExpressions;
using Unidecode.NET;

namespace StoryTeller.Data.Utils
{
    public static class SlugHelper
    {
        public static string ToSlug(this string inputStr)
        {
            if (string.IsNullOrWhiteSpace(inputStr))
                return string.Empty;

            // Step 1: Transliterate to ASCII
            string str = inputStr.Unidecode().ToLowerInvariant();

            // Step 2: Remove invalid characters
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            // Step 3: Normalize spaces/hyphens
            str = Regex.Replace(str, @"[\s-]+", " ").Trim();

            // Step 4: Replace spaces with hyphens
            str = Regex.Replace(str, @"\s", "-");

            // Step 5: Trim hyphens from ends
            str = str.Trim('-');

            return str;
        }
    }
}
