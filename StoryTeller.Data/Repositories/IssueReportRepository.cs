using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories
{
    public class IssueReportRepository : GenericRepository<IssueReport>, IIssueReportRepository
    {
        private readonly StoryTellerContext _context;

        public IssueReportRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
        }

        public async Task<List<IssueReport>> GetAllDetailAsync()
        {
            return await _dbSet
                .Include(ir => ir.User)
                .Where(ir => !ir.IsDeleted)
                .ToListAsync();
        }
        public async Task<PaginatedResult<IssueReport>> GetAllDetailAsPaginatedResultAsync(int page = 1, int pageSize = 10)
        {
            return await _dbSet
                .Include(ir => ir.User)
                .Where(ir => !ir.IsDeleted)
                .ToPaginatedResultAsync(page, pageSize);
        }

        public async Task<List<IssueReport>> GetByUserId(string userId)
        {
            return await _dbSet
                .Include(ir => ir.User)
                .Where(ir => ir.UserId == userId && !ir.IsDeleted)
                .ToListAsync();
        }

        public async Task<IssueReport?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(ir => ir.User)
                .Where(ir => !ir.IsDeleted)
                .FirstOrDefaultAsync(ir => ir.Id == id);
        }

        public async Task<List<IssueReport>> GetCommentReports(Guid commentId)
        {
            return await _dbSet
                .Include(ir => ir.User)
                .Where(ir => !ir.IsDeleted && ir.TargetType == IssueReportConst.TargetType.COMMENT && ir.TargetId == commentId.ToString())
                .ToListAsync();
        }

        public async Task<int> NumberOfUserReportedCount(Guid commentId)
        {
            var result = await _dbSet
                .Include(ir => ir.User)
                .Where(ir => !ir.IsDeleted && ir.TargetType == IssueReportConst.TargetType.COMMENT && ir.TargetId == commentId.ToString())
                .CountAsync();

            return result;
        }
    }
}
