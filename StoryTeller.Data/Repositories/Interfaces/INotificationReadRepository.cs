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
    public interface INotificationReadRepository : IGenericRepository<NotificationRead>
    {
        Task<List<Guid>> GetUnreadNotificationIdsAsync(IEnumerable<Guid> notificationIds, string userId, CancellationToken ct = default);
    }
}
