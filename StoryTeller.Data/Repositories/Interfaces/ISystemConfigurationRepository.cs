using StoryTeller.Data.Base;
using StoryTeller.Data.Entities;

namespace StoryTeller.Data.Repositories.Interfaces
{
    public interface ISystemConfigurationRepository : IGenericRepository<SystemConfiguration>
    {
        Task<string?> GetValueAsync(string key, CancellationToken ct = default);
        Task<int> GetIntAsync(string key, int defaultValue = 0, CancellationToken ct = default);
        Task<double> GetDoubleAsync(string key, double defaultValue = 0, CancellationToken ct = default);
        Task<List<SystemConfiguration>> GetAllConfigsAsync(CancellationToken ct = default);
        Task RefreshCacheAsync(CancellationToken ct = default);
        void InvalidateCache();
    }
}
