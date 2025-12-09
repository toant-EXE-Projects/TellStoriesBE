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
    public interface IIssueReportRepository : IGenericRepository<IssueReport>
    {
        Task<List<IssueReport>> GetAllDetailAsync();
        Task<PaginatedResult<IssueReport>> GetAllDetailAsPaginatedResultAsync(int page = 1, int pageSize = 10);
        Task<List<IssueReport>> GetByUserId(string userId);
        Task<IssueReport?> GetByIdAsync(Guid id);
        Task<List<IssueReport>> GetCommentReports(Guid commentId);
        Task<int> NumberOfUserReportedCount(Guid commentId);

    }
}
