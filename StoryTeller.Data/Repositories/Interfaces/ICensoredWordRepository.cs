using StoryTeller.Data.Base;
using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories.Interfaces
{
    public interface ICensoredWordRepository : IGenericRepository<CensoredWord>
    {
        public Task<HashSet<string>> GetCensoredWordsAsync(CancellationToken ct = default);
        public Task RefreshCacheAsync(CancellationToken ct = default);
        public void InvalidateCache();
        public Task<List<string>> GetExistingWordsAsync(IEnumerable<string> words, CancellationToken ct = default);
    }
}
