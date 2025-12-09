using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories
{
    public class CensoredWordRepository : GenericRepository<CensoredWord>, ICensoredWordRepository
    {
        private readonly StoryTellerContext _context;
        private HashSet<string> _cachedWords = new();
        private readonly SemaphoreSlim _cacheLock = new(1, 1);

        public CensoredWordRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
        }

        public async Task<HashSet<string>> GetCensoredWordsAsync(CancellationToken ct = default)
        {
            if (_cachedWords.Count == 0)
            {
                await _cacheLock.WaitAsync(ct);
                try
                {
                    if (_cachedWords.Count == 0)
                        await RefreshCacheAsync(ct);
                }
                finally
                {
                    _cacheLock.Release();
                }
            }

            return _cachedWords;
        }

        public async Task RefreshCacheAsync(CancellationToken ct = default)
        {
            var words = await _context.CensoredWords
                .Select(w => w.Word.ToLower())
                .ToListAsync(ct);

            _cachedWords = new HashSet<string>(words);
        }

        public void InvalidateCache() => _cachedWords.Clear();


        public async Task<List<string>> GetExistingWordsAsync(IEnumerable<string> words, CancellationToken ct = default)
        {
            var lowerWords = words.Select(w => w.ToLower());
            return await _context.CensoredWords
                .Where(w => lowerWords.Contains(w.Word.ToLower()))
                .Select(w => w.Word.ToLower())
                .ToListAsync(ct);
        }
    }
}
