using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Repositories.Interfaces;
using System.Collections.Concurrent;

namespace StoryTeller.Data.Repositories
{
    public class SystemConfigurationRepository : GenericRepository<SystemConfiguration>, ISystemConfigurationRepository
    {
        private readonly StoryTellerContext _context;
        private readonly SemaphoreSlim _cacheLock = new(1, 1);
        private ConcurrentDictionary<string, string> _cachedConfigs = new();

        public SystemConfigurationRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
        }

        public async Task<string?> GetValueAsync(string key, CancellationToken ct = default)
        {
            if (_cachedConfigs.IsEmpty)
            {
                await EnsureCacheAsync(ct);
            }

            return _cachedConfigs.TryGetValue(key, out var value) ? value : null;
        }

        public async Task<int> GetIntAsync(string key, int defaultValue = 0, CancellationToken ct = default)
        {
            var str = await GetValueAsync(key, ct);
            return int.TryParse(str, out var val) ? val : defaultValue;
        }

        public async Task<double> GetDoubleAsync(string key, double defaultValue = 0, CancellationToken ct = default)
        {
            var str = await GetValueAsync(key, ct);
            return double.TryParse(str, out var val) ? val : defaultValue;
        }

        public async Task<List<SystemConfiguration>> GetAllConfigsAsync(CancellationToken ct = default)
        {
            if (_cachedConfigs.IsEmpty)
            {
                await EnsureCacheAsync(ct);
            }
            return await _dbSet
                .OrderBy(x => x.Key)
                .Include(cb => cb.CreatedBy)
                .Include(cb => cb.UpdatedBy)
                .ToListAsync(ct);
        }

        private async Task EnsureCacheAsync(CancellationToken ct = default)
        {
            if (_cachedConfigs.IsEmpty)
            {
                await _cacheLock.WaitAsync(ct);
                try
                {
                    if (_cachedConfigs.IsEmpty)
                    {
                        await RefreshCacheAsync(ct);
                    }
                }
                finally
                {
                    _cacheLock.Release();
                }
            }
        }

        public async Task RefreshCacheAsync(CancellationToken ct = default)
        {
            var configs = await _context.SystemConfigurations.ToListAsync(ct);
            _cachedConfigs = new ConcurrentDictionary<string, string>(
                configs.ToDictionary(c => c.Key, c => c.Value));
        }

        public void InvalidateCache() => _cachedConfigs.Clear();
    }
}
