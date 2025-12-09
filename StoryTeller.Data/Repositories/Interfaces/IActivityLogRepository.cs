using StoryTeller.Data.Base;
using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories.Interfaces
{
    public interface IActivityLogRepository : IGenericRepository<ActivityLog>
    {
        Task<List<ActivityLog>?> GetAllDetailAsync();
        Task<List<ActivityLog>?> GetByUserId(string userId, DateTime getByDateFrom, DateTime getByDateTo, CancellationToken ct = default);
        Task<int> ClearLogAsync();
    }
}
