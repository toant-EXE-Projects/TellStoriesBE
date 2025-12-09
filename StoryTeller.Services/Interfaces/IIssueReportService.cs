using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Interfaces
{
    public interface IIssueReportService
    {
        Task<IssueReportDTO> GetByIdAsync(Guid id);
        Task<List<IssueReportDTO>> GetByUserIdAsync(string id);
        Task<List<IssueReportDTO>> GetReportedIssueByUserId(string id);
        Task<List<IssueReportDTO>> GetAllAsync();
        Task<PaginatedResult<IssueReportDTO>> GetAllAsPaginatedResultAsync(int page = 1, int pageSize = 10);
        Task<IssueReportDTO> CreateAsync(IssueReportCreateRequest activityLog, ApplicationUser user);
        Task<IssueReportDTO> ResolveAsync(Guid id, ApplicationUser user);
        Task<IssueReportDTO> DeleteAsync(Guid id, ApplicationUser user);
    }
}
