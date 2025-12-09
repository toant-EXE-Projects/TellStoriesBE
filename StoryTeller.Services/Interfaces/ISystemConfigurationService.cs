using StoryTeller.Data.Entities;
using StoryTeller.Services.Models.DTO;

namespace StoryTeller.Services.Interfaces
{
    public interface ISystemConfigurationService
    {
        Task<List<SystemConfigurationDTO>> GetAllConfigsAsync(CancellationToken ct = default);
        Task<string?> GetValueAsync(string key, CancellationToken ct = default);
        Task<int> GetIntAsync(string key, int defaultValue = 0, CancellationToken ct = default);
        Task<double> GetDoubleAsync(string key, double defaultValue = 0, CancellationToken ct = default);
        Task UpdateAsync(string key, string value, ApplicationUser user, CancellationToken ct = default);
        Task RefreshCacheAsync(CancellationToken ct = default);
    }
}
