using System.Text.RegularExpressions;

namespace StoryTeller.Data.Utils
{
    // http://james.newtonking.com/archive/2009/07/03/simple-net-profanity-filter
    // IList<string> censoredWords = new List<string>
    // {
    //  "gosh",
    //  "drat",
    //  "darn*",
    // };
    // darn -> ****
    // darnit -> ******
    public class CensoredException : Exception
    {
        public CensoredException(string message)
            : base(message) { }

        public CensoredException(string message, Exception inner)
            : base(message, inner) { }

        public List<string> MatchedWords { get; set; } = new();
    }

    public class Censor
    {
        public IList<string> CensoredWords { get; private set; }

        public Censor(IEnumerable<string> censoredWords)
        {
            if (censoredWords == null)
                throw new ArgumentNullException("censoredWords");

            CensoredWords = new List<string>(censoredWords);
        }

        /// <summary>
        /// Censors a given input string by replacing any words matching the defined censorship list.
        /// Optionally blocks the text entirely if any matches are found.
        /// </summary>
        /// <param name="text">The input text to be checked and censored.</param>
        /// <param name="blockIfMatch">
        /// If <c>true</c>, the method throws a <see cref="CensoredException"/> when any censored word is found.
        /// If <c>false</c>, the method replaces matched words with asterisks (e.g., "darn" → "****").
        /// </param>
        /// <returns>
        /// The censored version of the input text with inappropriate words replaced by asterisks.
        /// If <paramref name="blockIfMatch"/> is true and a match is found, the method throws and does not return.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
        /// <exception cref="CensoredException">Thrown when <paramref name="blockIfMatch"/> is true and censored words are found.</exception>
        public string CensorText(string text, bool blockIfMatch = false)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            string censoredText = text;
            var matchedWords = new List<string>();

            foreach (string censoredWord in CensoredWords)
            {
                string regularExpression = ToRegexPattern(censoredWord);

                if (Regex.IsMatch(censoredText, regularExpression, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    if (blockIfMatch)
                    {
                        matchedWords.Add(censoredWord);
                    }

                    censoredText = Regex.Replace(
                        censoredText,
                        regularExpression,
                        StarCensoredMatch,
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
                    );
                }
            }

            if (blockIfMatch && matchedWords.Count > 0)
            {
                throw new CensoredException("Text contains inappropriate language.")
                {
                    MatchedWords = matchedWords
                };
            }

            return censoredText;
        }

        private static string StarCensoredMatch(Match m)
        {
            string word = m.Captures[0].Value;

            return new string('*', word.Length);
        }

        private string ToRegexPattern(string wildcardSearch)
        {
            string regexPattern = Regex.Escape(wildcardSearch);

            regexPattern = regexPattern.Replace(@"\*", ".*?");
            regexPattern = regexPattern.Replace(@"\?", ".");

            if (regexPattern.StartsWith(".*?"))
            {
                regexPattern = regexPattern.Substring(3);
                regexPattern = @"(^\b)*?" + regexPattern;
            }

            regexPattern = @"\b" + regexPattern + @"\b";

            return regexPattern;
        }
    }
}
