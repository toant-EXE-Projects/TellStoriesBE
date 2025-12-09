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
    public class ActivityLogRepository : GenericRepository<ActivityLog>, IActivityLogRepository
    {
        private readonly StoryTellerContext _context;

        public ActivityLogRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
        }

        public async Task<List<ActivityLog>?> GetAllDetailAsync()
        {
            return await _dbSet
                .Include(al => al.User)
                .ToListAsync();
        }

        public async Task<List<ActivityLog>?> GetByUserId(string userId, DateTime getByDateFrom, DateTime getByDateTo, CancellationToken ct = default)
        {
            var from = getByDateFrom.Date;
            var to = getByDateTo >= DateTime.MaxValue ? getByDateTo.Date.AddDays(1).AddTicks(-1) : DateTime.MaxValue;

            return await _dbSet
                .Include(al => al.User)
                .Where(al => al.UserId == userId && al.CreatedDate >= from && al.CreatedDate <= to)
                .OrderByDescending(al => al.CreatedDate)
                .ToListAsync();
        }
        public async Task<int> ClearLogAsync()
        {
            return await _context.ActivityLogs
                .ExecuteDeleteAsync();
        }
    }
}
