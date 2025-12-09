using AutoMapper;
using StoryTeller.Data;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Logger;
using StoryTeller.Data.Utils;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;

namespace StoryTeller.Services.Implementations
{
    public class SystemConfigurationService : ISystemConfigurationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILoggerService _loggerService;

        public SystemConfigurationService(IUnitOfWork uow, IMapper mapper, ILoggerService loggerService)
        {
            _uow = uow;
            _mapper = mapper;
            _loggerService = loggerService;
        }

        public async Task<List<SystemConfigurationDTO>> GetAllConfigsAsync(CancellationToken ct = default)
        {
            var configs = await _uow.SystemConfigurations.GetAllConfigsAsync(ct);
            var mapped = _mapper.Map<List<SystemConfigurationDTO>>(configs);
            return mapped;
        }

        public Task<string?> GetValueAsync(string key, CancellationToken ct = default)
            => _uow.SystemConfigurations.GetValueAsync(key, ct);

        public Task<int> GetIntAsync(string key, int defaultValue = 0, CancellationToken ct = default)
            => _uow.SystemConfigurations.GetIntAsync(key, defaultValue, ct);

        public Task<double> GetDoubleAsync(string key, double defaultValue = 0, CancellationToken ct = default)
            => _uow.SystemConfigurations.GetDoubleAsync(key, defaultValue, ct);

        public async Task UpdateAsync(string key, string value, ApplicationUser user, CancellationToken ct = default)
        {
            var existing = await _uow.SystemConfigurations
                .FirstOrDefaultAsync(c => c.Key == key, ct);

            if (existing == null)
            {
                var newConfig = new SystemConfiguration
                {
                    Key = key,
                    Value = value
                };

                await _uow.SystemConfigurations.CreateAsync(newConfig, user, ct);
            }
            else
            {
                existing.Value = value;
                await _uow.SystemConfigurations.UpdateAsync(existing, user, ct);
            }

            await _uow.SaveChangesAsync(ct);

            await _uow.SystemConfigurations.RefreshCacheAsync(ct);

            _loggerService.LogInfo($"System config updated [({user.Id}) - {user.Email}]: {key} = {value}");
        }

        public Task RefreshCacheAsync(CancellationToken ct = default)
            => _uow.SystemConfigurations.RefreshCacheAsync(ct);

    }
}
