using AutoMapper;
using Azure.Core;
using StoryTeller.Data;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Logger;
using StoryTeller.Data.Models;
using StoryTeller.Data.Utils;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Implementations
{
    public class CensorService : ICensorService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILoggerService _loggerService;

        public CensorService(IUnitOfWork uow, IMapper mapper, ILoggerService loggerService)
        {
            _uow = uow;
            _mapper = mapper;
            _loggerService = loggerService;
        }

        public async Task<string> FilterContentAsync(string text, bool block = false, CancellationToken ct = default)
        {
            var words = await _uow.CensoredWords.GetCensoredWordsAsync(ct);
            var censor = new Censor(words);
            return censor.CensorText(text, block);
        }
        public async Task<List<CensoredWordDTO>> GetCensoredWordsListAsync(CancellationToken ct = default)
        {
            var res = _mapper.Map<List<CensoredWordDTO>>(await _uow.CensoredWords.GetAllAsync(ct));
            return res;
        }

        public async Task<bool> AddCensoredWordsAsync(IEnumerable<string> words, ApplicationUser user, CancellationToken ct = default)
        {
            var lowerWords = words
                .Select(w => w.Trim().ToLower())
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Distinct()
                .ToList();

            if (!lowerWords.Any())
                return false;

            var existingWords = await _uow.CensoredWords.GetExistingWordsAsync(lowerWords, ct);

            var newWords = lowerWords
                .Except(existingWords)
                .Select(w => new CensoredWord
                {
                    Word = w,
                    IsWildcard = w.Contains('*')
                })
                .ToList();

            if (!newWords.Any())
                return false;

            await _uow.CensoredWords.CreateRangeAsync(newWords, user, ct);
            await _uow.SaveChangesAsync(ct);
            _uow.CensoredWords.InvalidateCache();
            await _uow.CensoredWords.RefreshCacheAsync(ct);
            return true;
        }

        public async Task<bool> DeleteCensoredWordsAsync(IEnumerable<Guid> wordIds, CancellationToken ct = default)
        {
            var ids = wordIds.Distinct().ToList();

            if (!ids.Any())
                return false;

            var wordsToDelete = await _uow.CensoredWords.FindAsync(w => ids.Contains(w.Id), ct);

            if (!wordsToDelete.Any())
                return false;

            _uow.CensoredWords.RemoveRange(wordsToDelete);
            await _uow.SaveChangesAsync(ct);

            _uow.CensoredWords.InvalidateCache();
            await _uow.CensoredWords.RefreshCacheAsync(ct);

            return true;
        }

    }
}
