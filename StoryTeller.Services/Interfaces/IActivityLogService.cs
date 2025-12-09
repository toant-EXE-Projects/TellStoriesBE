using Azure;
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
    public interface IActivityLogService
    {
        Task<ActivityLogDTO> GetByIdAsync(Guid id);
        Task<List<ActivityLogDTO>> GetByUserIdAsync(string id, DateTime? getByDateFrom, DateTime? getByDateTo, int page, int pageSize, CancellationToken ct = default);
        Task<List<ActivityLogDTO>> GetAllAsync();
        Task<ActivityLogDTO> CreateAsync(ActivityLogCreateRequest activityLog, ApplicationUser user);
        Task<bool> ClearLogAsync();
    }
}
