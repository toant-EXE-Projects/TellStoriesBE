using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories
{
    public class UserSubscriptionRepository : GenericRepository<UserSubscription>, IUserSubscriptionRepository
    {
        private readonly StoryTellerContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UserSubscriptionRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }
        public async Task<List<string>> GetSubscriptionNamesAsync()
        {
            return await _dbSet
                .Include(u => u.Subscription)
                .Where(u => (bool)u.Subscription.IsActive!)
                .Select(u => u.Subscription.Name)
                .Distinct()
                .ToListAsync();
        }
        public async Task<UserSubscription?> GetUserActiveSubscription(string userId, CancellationToken ct)
        {
            var now = _dateTimeProvider.GetSystemCurrentTime();
            var subs = await _dbSet
                .Include(u => u.Subscription)
                .Where(x => x.UserId == userId)
                .ToListAsync(ct);

            return subs.FirstOrDefault(x => x.IsActive(now));
        }

        public async Task<List<UserSubscription>> GetExpiringSubscriptionsAsync(DateTime now, DateTime maxDate, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(u => u.Subscription)
                .Where(s => s.EndDate.HasValue &&
                     s.EndDate.Value.Date >= now &&
                     s.EndDate.Value.Date <= maxDate &&
                    (s.Status == null || s.Status == SubscriptionConstants.StatusActive) &&
                    (s.HasNotified == null || s.HasNotified == false)
                )
                .ToListAsync(ct);
        }

        public async Task<List<UserSubscription>> SearchSubscriberQuery(string query)
        {
            query = query.ToLower();

            var queryResult = _dbSet
                .Include(us => us.Subscription)
                .Include(us => us.User)
                .Where(us =>
                    us.Status != SubscriptionConstants.StatusExpired &&
                    !us.IsDeleted &&
                    (
                        (us.User.DisplayName != null && us.User.DisplayName.ToLower().Contains(query)) ||
                        us.Subscription.Name.ToLower().Contains(query)
                    )
                );
            return await queryResult.ToListAsync();        
        }
    }
}
