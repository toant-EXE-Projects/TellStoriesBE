using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StoryTeller.Data.Constants;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Repositories;
using StoryTeller.Data.Repositories.Interfaces;

namespace StoryTeller.Data.Seed
{
    public static class SeedSystemConfigs
    {
        private static readonly List<(string Key, string Value)> DefaultSystemConfigs = new()
        {
            (
                SystemConst.Keys.Login_DailyReward_Points, 
                SystemConst.Values.Login_DailyReward_Points.ToString()
            ),
            (
                SystemConst.Keys.StoryPublish_MaxPendingRequests_Default,
                SystemConst.Values.StoryPublish_MaxPendingRequests_Default.ToString()
            ),
            (
                SystemConst.Keys.StoryPublish_MaxPendingRequests_Tier1, 
                SystemConst.Values.StoryPublish_MaxPendingRequests_Tier1.ToString()
            ),
            (
                SystemConst.Keys.Subscription_Background_CheckExpiration_IntervalMinutes, 
                SystemConst.Values.Subscription_Background_CheckExpiration_IntervalMinutes.ToString()
            ),
            (
                SystemConst.Keys.Subscription_Background_SendEmailUpcomingExpiration_IntervalMinutes, 
                SystemConst.Values.Subscription_Background_SendEmailUpcomingExpiration_IntervalMinutes.ToString()
            ),
            (
                SystemConst.Keys.Subscription_Background_SendEmailUpcomingExpiration_DaysBeforeExpiration, 
                SystemConst.Values.Subscription_Background_SendEmailUpcomingExpiration_DaysBeforeExpiration.ToString()
            ),
            (
                SystemConst.Keys.BillingRecord_Background_CheckRecords_IntervalMinutes, 
                SystemConst.Values.BillingRecord_Background_CheckRecords_IntervalMinutes.ToString()
            ),
            (
                SystemConst.Keys.BillingRecord_Background_MarkRecordAsFailedIfNotPaidInMinutes, 
                SystemConst.Values.BillingRecord_Background_MarkRecordAsFailedIfNotPaidInMinutes.ToString()
            ),
        };
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoryTellerContext>();
            var sysConfigRepo = scope.ServiceProvider.GetRequiredService<ISystemConfigurationRepository>();

            foreach (var (key, value) in DefaultSystemConfigs)
            {
                if (!await context.SystemConfigurations.AnyAsync(c => c.Key == key))
                {
                    await sysConfigRepo.CreateAsync(new SystemConfiguration
                    {
                        Key = key,
                        Value = value,
                    }, null);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
