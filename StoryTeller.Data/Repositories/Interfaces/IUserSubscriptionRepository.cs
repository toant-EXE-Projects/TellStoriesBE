using StoryTeller.Data.Base;
using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories.Interfaces
{
    public interface IUserSubscriptionRepository : IGenericRepository<UserSubscription>
    {
        Task<List<string>> GetSubscriptionNamesAsync();
        Task<UserSubscription?> GetUserActiveSubscription(string userId, CancellationToken ct);
        Task<List<UserSubscription>> SearchSubscriberQuery(string query);
        Task<List<UserSubscription>> GetExpiringSubscriptionsAsync(DateTime now, DateTime maxDate, CancellationToken ct = default);
    }
}
