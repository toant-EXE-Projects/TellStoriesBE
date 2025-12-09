using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoryTeller.Data.Utils
{
    public class StringUtils
    {
        public static string NormalizeToAscii(string input)
        {
            if (input == null) return string.Empty;
            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).ToLower();
        }

        public static string ToUpperFirstAndAfterSpaceChar(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            input = input.Replace(input[0], char.ToUpper(input[0]));

            for (int i = input.IndexOf(' '); i > -1; i = input.IndexOf(' ', i + 1))
            {
                input = input.Replace(input[i+1], char.ToUpper(input[i + 1]));
            }

            return input;
        }

        /// <summary>
        /// Splits a given text into smaller chunks that do not exceed the specified maximum length.
        /// 
        /// The algorithm prefers natural break points:
        /// 1. First, it splits the text into sentences using punctuation (. ! ?).
        /// 2. If a sentence is longer than <paramref name="maxChunkLength"/>, it is further split into words.
        /// 3. Words are grouped together until the chunk reaches the maximum length, then a new chunk is started.
        /// 4. Sentences shorter than <paramref name="maxChunkLength"/> are kept together whenever possible.
        /// </summary>
        /// <param name="text">The input text to split.</param>
        /// <param name="maxChunkLength">The maximum number of characters allowed per chunk (default: 500).</param>
        /// <returns>A list of strings split into chunks</returns>
        public static List<string> SplitTextIntoChunks(string text, int maxChunkLength = 500)
        {
            var sentences = Regex.Split(text, @"(?<=[.!?])\s+");
            var chunks = new List<string>();
            var current = new StringBuilder();

            foreach (var sentence in sentences)
            {
                // Sentence is greater than maxChunkLength chunk by words instead.
                // You check if adding this word would exceed the maxChunkLength.
                // Push the current buffer (current) into chunks.
                // Clear the buffer.
                // Append the word.
                if (sentence.Length > maxChunkLength)
                {
                    var words = sentence.Split(' ');
                    foreach (var word in words)
                    {
                        if (current.Length + word.Length + 1 > maxChunkLength)
                        {
                            if (current.Length > 0)
                            {
                                chunks.Add(current.ToString().Trim());
                                current.Clear();
                            }
                        }
                        current.Append(word + " ");
                    }
                }
                // Chunk by sentence
                else
                {
                    if (current.Length + sentence.Length > maxChunkLength)
                    {
                        chunks.Add(current.ToString().Trim());
                        current.Clear();
                    }
                    current.Append(sentence + " ");
                }
            }

            if (current.Length > 0)
                chunks.Add(current.ToString().Trim());

            return chunks;
        }
    }
}
