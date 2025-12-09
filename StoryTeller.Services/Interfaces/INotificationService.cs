using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Services.Implementations;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Interfaces
{
    public interface INotificationService
    {
        public Task<NotificationDTO> NotifyAsync(NotificationSendRequest request, ApplicationUser user, CancellationToken ct = default);
        public Task<NotificationDTO> ReadNotification(Guid notifId, ApplicationUser user, CancellationToken ct = default);
        public Task<bool> MarkAsRead(Guid notifId, ApplicationUser user, CancellationToken ct = default);
        public Task<bool> MarkAllAsRead(ApplicationUser user, CancellationToken ct = default);
        public Task<PaginatedResult<NotificationDTO>> GetUserNotificationsPagedAsync(
            string userId,
            string? searchQuery,
            bool onlyUnread = false,
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default
        );
        public Task<PaginatedResult<NotificationDTO>> GetMySentNotificationsAsync(
            ApplicationUser user,
            string? searchQuery,
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default
        );
    }
}
