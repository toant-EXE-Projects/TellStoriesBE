using StoryTeller.Data.Entities;
using StoryTeller.Services.Models.DTO;

namespace StoryTeller.Services.Interfaces
{
    public interface ICensorService
    {
        public Task<string> FilterContentAsync(string text, bool block = false, CancellationToken ct = default);
        public Task<bool> AddCensoredWordsAsync(IEnumerable<string> words, ApplicationUser user, CancellationToken ct = default);
        public Task<bool> DeleteCensoredWordsAsync(IEnumerable<Guid> wordIds, CancellationToken ct = default);
        public Task<List<CensoredWordDTO>> GetCensoredWordsListAsync(CancellationToken ct = default);
    }
}
