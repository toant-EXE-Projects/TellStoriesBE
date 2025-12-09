using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
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
    public class BillingRecordRepository : GenericRepository<BillingRecord>, IBillingRecordRepository
    {
        private readonly StoryTellerContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public BillingRecordRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<BillingRecord?> GetByBillingId(string billingId, CancellationToken ct)
        {
            return await _dbSet.Where(br => br.BillingId == billingId).FirstOrDefaultAsync(ct);
        }

        public async Task<List<BillingRecord>> GetByUserId(string userId)
        {
            return await _dbSet
                .Include(br => br.Subscription)
                .Where(br => br.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<BillingRecord>> QueryBillingRecords(string? query, CancellationToken ct)
        {

            query = query == null ? string.Empty : query.ToLower();

            return await _dbSet
                .Include(br => br.User)
                .Include(br => br.Subscription)
                .Where(br =>
                    !br.IsDeleted &&
                    (
                        (br.Subscription != null && br.Subscription.Name.ToLower().Contains(query)) ||
                        (br.User != null &&
                            (
                            br.User.DisplayName!.ToLower().Contains(query) ||
                            br.User.Email!.ToLower().Contains(query)
                            )
                        ) ||
                        br.PaymentMethod.ToLower().Contains(query) ||
                        br.Status.ToLower().Contains(query)
                    )
                )
                .ToListAsync();
        }
    }
}
