using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories
{
    public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
    {
        private readonly StoryTellerContext _context;

        public SubscriptionRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
        }

        public async Task<List<Subscription>> GetAllNotDeletedAsync()
        {
            return await _dbSet
                .Where(s => !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Subscription>> GetAllActiveAsync(SubscriptionPurchaseMethod? method, CancellationToken ct = default)
        {
            var query = _dbSet
                .Where(s =>
                    s.IsActive != null &&
                    (bool)s.IsActive &&
                    !s.IsDeleted &&
                    (method == null || s.PurchaseMethod == method)
                );

            var res = await query.ToListAsync(ct);
            return res;
        }
    }
}
