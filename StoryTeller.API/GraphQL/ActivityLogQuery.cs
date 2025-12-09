using HotChocolate.Authorization;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;

namespace StoryTeller.API.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    [Authorize(Policy = Policies.StaffOnly)]
    public class ActivityLogQuery
    {
        public async Task<List<ActivityLogDTO>> GetActivityLogs([Service] IActivityLogService activityLogService)
            => await activityLogService.GetAllAsync();

        public async Task<ActivityLogDTO?> GetActivityLogById(string id, [Service] IActivityLogService activityLogService)
            => await activityLogService.GetByIdAsync(new Guid(id));
    }
}
