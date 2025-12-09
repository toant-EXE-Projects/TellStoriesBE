using StoryTeller.Data.Base;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories.Interfaces
{
    public interface ISubscriptionRepository : IGenericRepository<Subscription>
    {
        Task<List<Subscription>> GetAllNotDeletedAsync();
        Task<List<Subscription>> GetAllActiveAsync(SubscriptionPurchaseMethod? method = null, CancellationToken ct = default);

    }
}
