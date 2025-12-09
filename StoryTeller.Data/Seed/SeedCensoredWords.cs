using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;

namespace StoryTeller.Data.Seed
{
    public class SeedCensoredWords
    {
        //https://github.com/dsojevic/profanity-list/blob/main/en.txt
        //https://github.com/blue-eyes-vn/vietnamese-offensive-words/blob/main/vn_offensive_words.txt
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoryTellerContext>();
            var dateTimeProvider = serviceProvider.GetRequiredService<IDateTimeProvider>();

            if (!await context.CensoredWords.AnyAsync())
            {
                var wordList = CensorWordList.All;

                var existingWords = await context.CensoredWords
                    .Select(w => w.Word.ToLower())
                    .ToListAsync();

                var existingWordSet = new HashSet<string>(existingWords);

                var newWords = wordList
                    .Where(word => !existingWordSet.Contains(word.ToLower()))
                    .Select(word => new CensoredWord
                    {
                        Id = Guid.NewGuid(),
                        Word = word,
                        IsWildcard = word.Contains('*'),
                        CreatedDate = dateTimeProvider.GetSystemCurrentTime(),
                    })
                    .ToList();

                if (newWords.Any())
                {
                    await context.CensoredWords.AddRangeAsync(newWords);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
