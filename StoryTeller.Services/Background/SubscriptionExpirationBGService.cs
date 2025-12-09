using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Logger;
using StoryTeller.Services.Implementations;
using StoryTeller.Services.Interfaces;

namespace StoryTeller.Services.Background
{
    public class SubscriptionExpirationBGService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerService _logger;
        private readonly IBackgroundTaskToggle _bgTaskToggle;
        private readonly string LogPrefix = "[SubscriptionExpirationBGService]";

        public SubscriptionExpirationBGService(
            IServiceProvider serviceProvider,
            ILoggerService logger,
            IBackgroundTaskToggle bgTaskToggle)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _bgTaskToggle = bgTaskToggle;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInfo($"{LogPrefix} started.");

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (!_bgTaskToggle.SubscriptionExpirationBGServiceEnabled)
                    {
                        _logger.LogInfo($"{LogPrefix} Service is disabled, skipping.");
                        await Task.Delay(TimeSpan.FromMinutes(1), ct);
                        continue;
                    }

                    await using var scope = _serviceProvider.CreateAsyncScope();
                    var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
                    var systemConfigurationService = scope.ServiceProvider.GetRequiredService<ISystemConfigurationService>();

                    int updated = await subscriptionService.MarkExpiredSubscriptionsAsync(ct);
                    if (updated > 0)
                        _logger.LogInfo($"{LogPrefix} Updated {updated} expired subscriptions.");

                    var sysConfig_intervalMinutes = await systemConfigurationService.GetIntAsync(
                        SystemConst.Keys.Subscription_Background_CheckExpiration_IntervalMinutes,
                        SystemConst.Values.Subscription_Background_CheckExpiration_IntervalMinutes,
                        ct
                    );
                    await Task.Delay(TimeSpan.FromMinutes(sysConfig_intervalMinutes), ct);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{LogPrefix} Unhandled error in loop.", ex);
                    await Task.Delay(TimeSpan.FromMinutes(1), ct);
                }
            }

            _logger.LogInfo($"{LogPrefix} stopping.");
        }
    }
}
