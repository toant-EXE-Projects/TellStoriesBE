using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;

namespace StoryTeller.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationService(IUnitOfWork uow, 
            IDateTimeProvider dateTimeProvider,
            IMapper mapper,
            UserManager<ApplicationUser> userManager
        )
        {
            _uow = uow;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
            _userManager = userManager;
        }

        public async Task<NotificationDTO> NotifyAsync(NotificationSendRequest request, ApplicationUser user, CancellationToken ct = default)
        {
            if (!string.IsNullOrWhiteSpace(request.UserId))
            {
                var targetUser = await _userManager.FindByIdAsync(request.UserId);
                if (targetUser == null)
                    throw new InvalidOperationException("Target user does not exist.");
            }

            var notification = _mapper.Map<Notification>(request);

            notification.SentAt = _dateTimeProvider.GetSystemCurrentTime();
            if (string.IsNullOrWhiteSpace(notification.Sender))
            {
                notification.Sender = user.DisplayName!;
            }

            await _uow.Notifications.CreateAsync(notification, user, ct);
            await _uow.SaveChangesAsync(ct);
            var res = _mapper.Map<NotificationDTO>(notification);
            return res;
        }

        public async Task<bool> MarkAsRead(Guid notifId, ApplicationUser user, CancellationToken ct = default)
        {
            var notif = await _uow.Notifications.GetByIdAsync(notifId, ct);
            if (notif == null) throw new NotFoundException("Notification not found.");

            // ensure this user is target if not broadcast
            if (notif.UserId != null && notif.UserId != user.Id)
                throw new InvalidOperationException("You cannot access this notification.");

            // check if already read
            var existingRead = await _uow.NotificationReads
                .FirstOrDefaultAsync(r => r.NotificationId == notifId && r.UserId == user.Id, ct);

            if (existingRead == null)
            {
                var read = new NotificationRead
                {
                    NotificationId = notifId,
                    UserId = user.Id,
                    ReadAt = _dateTimeProvider.GetSystemCurrentTime()
                };
                await _uow.NotificationReads.CreateAsync(read, user, ct);
                await _uow.SaveChangesAsync(ct);
            }

            return true;
        }

        public async Task<bool> MarkAllAsRead(ApplicationUser user, CancellationToken ct = default)
        {
            var unreadNotifs = await _uow.Notifications.FindAsync(
                n => (n.UserId == null || n.UserId == user.Id), ct);

            var unreadIds = await _uow.NotificationReads
                .GetUnreadNotificationIdsAsync(unreadNotifs.Select(n => n.Id), user.Id, ct);

            if (!unreadIds.Any())
                return false;

            foreach (var notifId in unreadIds)
            {
                var read = new NotificationRead
                {
                    NotificationId = notifId,
                    UserId = user.Id,
                    ReadAt = _dateTimeProvider.GetSystemCurrentTime()
                };
                await _uow.NotificationReads.CreateAsync(read, user, ct);
            }

            await _uow.SaveChangesAsync(ct);
            return true;
        }

        public async Task<NotificationDTO> ReadNotification(Guid notifId, ApplicationUser user, CancellationToken ct = default)
        {
            var notif = await _uow.Notifications.GetByIdAsync(notifId, ct);
            if (notif == null) throw new NotFoundException("Notification not found.");

            if (notif.UserId != null && notif.UserId != user.Id)
                throw new InvalidOperationException("You cannot access this notification.");

            var mappedResult = _mapper.Map<NotificationDTO>(notif);

            await MarkAsRead(notif.Id, user, ct);

            mappedResult.IsRead = true;

            return mappedResult;
        }

        public async Task<PaginatedResult<NotificationDTO>> GetUserNotificationsPagedAsync(
            string userId,
            string? searchQuery,
            bool onlyUnread = false,
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default
        )
        {
            var res = await _uow.Notifications.GetUserNotificationsPagedAsync(userId, searchQuery, onlyUnread, page, pageSize, ct);
            var mappedResult = _mapper.Map<PaginatedResult<NotificationDTO>>(res);
            return mappedResult;
        }

        public async Task<PaginatedResult<NotificationDTO>> GetMySentNotificationsAsync(
            ApplicationUser user,
            string? searchQuery,
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default
        )
        {
            var paged = await _uow.Notifications.GetSentNotificationsAsync(user, searchQuery, page, pageSize, ct);

            return _mapper.Map<PaginatedResult<NotificationDTO>>(paged);
        }
    }
}
