using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Logger;
using StoryTeller.Services.Interfaces;

namespace StoryTeller.Services.Background
{
    public class SubscriptionEmailReminderBGService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerService _logger;
        private readonly IBackgroundTaskToggle _bgTaskToggle;
        private readonly string LogPrefix = "[SubscriptionEmailReminderBGService]";

        public SubscriptionEmailReminderBGService(
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
                    if (!_bgTaskToggle.SubscriptionEmailReminderEnabled)
                    {
                        _logger.LogInfo($"{LogPrefix} is disabled, skipping.");
                        await Task.Delay(TimeSpan.FromMinutes(1), ct);
                        continue;
                    }

                    await using var scope = _serviceProvider.CreateAsyncScope();
                    var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
                    var systemConfigurationService = scope.ServiceProvider.GetRequiredService<ISystemConfigurationService>();

                    var sysConfig_daysBeforeExpiration = await systemConfigurationService.GetIntAsync(
                        SystemConst.Keys.Subscription_Background_SendEmailUpcomingExpiration_DaysBeforeExpiration,
                        SystemConst.Values.Subscription_Background_SendEmailUpcomingExpiration_DaysBeforeExpiration,
                        ct
                    );
                    int emailSent = await subscriptionService.NotifyUpcomingExpirationsAsync(
                        sysConfig_daysBeforeExpiration, ct);
                    if (emailSent > 0)
                        _logger.LogInfo($"{LogPrefix} {emailSent} email sent.");

                    var sysConfig_intervalMinutes = await systemConfigurationService.GetIntAsync(
                        SystemConst.Keys.Subscription_Background_SendEmailUpcomingExpiration_IntervalMinutes,
                        SystemConst.Values.Subscription_Background_SendEmailUpcomingExpiration_IntervalMinutes,
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
