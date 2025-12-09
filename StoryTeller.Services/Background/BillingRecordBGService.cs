using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Logger;
using StoryTeller.Services.Interfaces;

namespace StoryTeller.Services.Background
{
    public class BillingRecordBGService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerService _logger;
        private readonly IBackgroundTaskToggle _bgTaskToggle;
        private readonly string LogPrefix = "[BillingRecordBGService]";

        public BillingRecordBGService(
            IServiceProvider serviceProvider,
            ILoggerService logger,
            IBackgroundTaskToggle bgTaskToggle
        )
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
                    if (!_bgTaskToggle.BillingRecordsEnabled)
                    {
                        _logger.LogInfo($"{LogPrefix} is disabled, skipping.");
                        await Task.Delay(TimeSpan.FromMinutes(1), ct);
                        continue;
                    }

                    await using var scope = _serviceProvider.CreateAsyncScope();
                    var subscriptionService = scope.ServiceProvider.GetRequiredService<IBillingRecordService>();
                    var systemConfigurationService = scope.ServiceProvider.GetRequiredService<ISystemConfigurationService>();

                    var sysConfig_markRecordAsFailedIfNotPaidInMinutes = await systemConfigurationService.GetIntAsync(
                        SystemConst.Keys.BillingRecord_Background_MarkRecordAsFailedIfNotPaidInMinutes,
                        SystemConst.Values.BillingRecord_Background_MarkRecordAsFailedIfNotPaidInMinutes,
                        ct
                    );
                    int updated = await subscriptionService.MarkExpiredRecordsAsync(
                        sysConfig_markRecordAsFailedIfNotPaidInMinutes,
                        ct);
                    if (updated > 0)
                        _logger.LogInfo($"{LogPrefix} Updated {updated} expired records.");

                    var sysConfig_intervalMinutes = await systemConfigurationService.GetIntAsync(
                        SystemConst.Keys.BillingRecord_Background_CheckRecords_IntervalMinutes,
                        SystemConst.Values.BillingRecord_Background_CheckRecords_IntervalMinutes,
                        ct
                    );
                    //Console.WriteLine($"BillingRecord_Background_CheckRecords_IntervalMinutes: {sysConfig_intervalMinutes}");
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
