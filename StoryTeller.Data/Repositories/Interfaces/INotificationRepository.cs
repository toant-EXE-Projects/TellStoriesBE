using StoryTeller.Data.Base;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        public Task<PaginatedResult<Notification>> GetUserNotificationsPagedAsync(
            string userId,
            string? searchQuery,
            bool onlyUnread = false,
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default
        );
        public Task<PaginatedResult<Notification>> GetSentNotificationsAsync(
            ApplicationUser user,
            string? searchQuery,
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default
        );
    }
}
