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
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly StoryTellerContext _context;

        public NotificationRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Notification>> GetUserNotificationsPagedAsync(
            string userId,
            string? searchQuery,
            bool onlyUnread = false,
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default
        )
        {
            // also get system wide notification
            var query = _dbSet
                .Where(n => 
                    n.UserId == userId ||
                    n.UserId == null
                );

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                query = query.Where(n =>
                    n.Title.ToLower().Contains(searchQuery) ||
                    n.Message.ToLower().Contains(searchQuery) ||
                    n.Sender.ToLower().Contains(searchQuery) ||
                    n.Type.ToLower().Contains(searchQuery)
                );
            }

            if (onlyUnread)
            {
                query = query.Where(n =>
                    !_context.NotificationReads
                        .Any(r => r.NotificationId == n.Id && r.UserId == userId)
                );
            }

            var items = await query
                .OrderByDescending(n => n.SentAt)
                .ToPaginatedResultAsync(page, pageSize, ct);

            // For DTO
            foreach (var n in items.Items)
            {
                n.IsRead = await _context.NotificationReads
                    .AnyAsync(r => r.NotificationId == n.Id && r.UserId == userId, ct);
            }

            return items;
        }

        public async Task<PaginatedResult<Notification>> GetSentNotificationsAsync(
            ApplicationUser user,
            string? searchQuery,
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default
        )
        {
            var query = _dbSet
                .Where(n =>
                    n.Sender == user.DisplayName ||
                    (n.CreatedBy != null && n.CreatedBy.Id == user.Id)
                );

            if (searchQuery != null && !string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                query = query.Where(n =>
                    n.Title.ToLower().Contains(searchQuery) ||
                    n.Message.ToLower().Contains(searchQuery) ||
                    n.Sender.ToLower().Contains(searchQuery) ||
                    (n.User != null && n.User.DisplayName != null && n.User.DisplayName.ToLower().Contains(searchQuery)) ||
                    (n.User != null && n.User.Email != null && n.User.Email.ToLower().Contains(searchQuery)) ||
                    n.Type.ToLower().Contains(searchQuery)
                );
            }

            var items = await query
                .OrderByDescending(n => n.SentAt)
                .ToPaginatedResultAsync(page, pageSize, ct);

            return items;
        }
    }
}
