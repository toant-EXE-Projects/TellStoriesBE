using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoryTeller.Data.Base;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Data.Repositories.Interfaces;
using StoryTeller.Data.Utils;

namespace StoryTeller.Data.Repositories
{
    public class NotificationReadRepository : GenericRepository<NotificationRead>, INotificationReadRepository
    {
        private readonly StoryTellerContext _context;

        public NotificationReadRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
        }

        public async Task<List<Guid>> GetUnreadNotificationIdsAsync(IEnumerable<Guid> notificationIds, string userId, CancellationToken ct = default)
        {
            var readIds = await _dbSet
                .Where(r => notificationIds.Contains(r.NotificationId) && r.UserId == userId)
                .Select(r => r.NotificationId)
                .ToListAsync(ct);

            return notificationIds.Except(readIds).ToList();
        }
    }
}
