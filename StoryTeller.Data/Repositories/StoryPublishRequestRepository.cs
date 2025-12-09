using Azure;
using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Data.Models;
using StoryTeller.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories
{
    public class StoryPublishRequestRepository : GenericRepository<StoryPublishRequest>, IStoryPublishRequestRepository
    {
        private readonly StoryTellerContext _context;

        public StoryPublishRequestRepository(
            StoryTellerContext context,
            IDateTimeProvider dateTimeProvider)
            : base(context, dateTimeProvider)
        {
            _context = context;
        }

        private IQueryable<StoryPublishRequest> BaseQuery =>
            _dbSet
                .Include(r => r.Story)
                .Include(r => r.CreatedBy)
                .Where(r => !r.IsDeleted)
                .OrderByDescending(r => r.CreatedDate);

        public IQueryable<StoryPublishRequest> GetQuery()
        {
            return BaseQuery;
        }

        public Task<PaginatedResult<StoryPublishRequest>> GetAllPaginatedAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            return BaseQuery.ToPaginatedResultAsync(page, pageSize, ct);
        }

        public Task<PaginatedResult<StoryPublishRequest>> GetPendingPaginatedAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            return BaseQuery
                .Where(r => r.Status == StoryPublishRequestStatus.Pending)
                .ToPaginatedResultAsync(page, pageSize, ct);
        }

        public Task<PaginatedResult<StoryPublishRequest>> GetRejectedPaginatedAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            return BaseQuery
                .Where(r => r.Status == StoryPublishRequestStatus.Rejected)
                .ToPaginatedResultAsync(page, pageSize, ct);
        }

        public Task<PaginatedResult<StoryPublishRequest>> GetUserRequestPaginatedAsync(ApplicationUser user, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            return BaseQuery
                .Where(r => r.CreatedBy == user)
                .ToPaginatedResultAsync(page, pageSize, ct);
        }

        public Task<List<StoryPublishRequest>> GetUserPendingRequestAsync(ApplicationUser user, CancellationToken ct = default)
        {
            return BaseQuery
                .Where(r => 
                    r.CreatedBy == user &&
                    r.Status == StoryPublishRequestStatus.Pending
                )
                .ToListAsync(ct);
        }

        public async Task CancelRequestAsync(Guid requestId, ApplicationUser user, CancellationToken ct = default)
        {
            var request = await _dbSet.FirstOrDefaultAsync(r =>
                r.Id == requestId,
                ct
            );

            if (request is null)
                throw new InvalidOperationException("Request not found or already processed.");

            if (request.Status != StoryPublishRequestStatus.Pending || request.IsDeleted)
                throw new InvalidOperationException("Request already cancelled");
            
            request.Status = StoryPublishRequestStatus.Cancelled;
            await SoftRemove(request, user);
        }
    }
}
