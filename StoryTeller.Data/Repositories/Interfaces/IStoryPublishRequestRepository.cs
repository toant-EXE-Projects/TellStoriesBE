using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories.Interfaces
{
    public interface IStoryPublishRequestRepository : IGenericRepository<StoryPublishRequest>
    {
        public IQueryable<StoryPublishRequest> GetQuery();
        public Task<PaginatedResult<StoryPublishRequest>> GetAllPaginatedAsync(int page = 1, int pageSize = 10, CancellationToken ct = default);

        public Task<PaginatedResult<StoryPublishRequest>> GetPendingPaginatedAsync(int page = 1, int pageSize = 10, CancellationToken ct = default);

        public Task<PaginatedResult<StoryPublishRequest>> GetRejectedPaginatedAsync(int page = 1, int pageSize = 10, CancellationToken ct = default);
        public Task<PaginatedResult<StoryPublishRequest>> GetUserRequestPaginatedAsync(ApplicationUser user, int page = 1, int pageSize = 10, CancellationToken ct = default);
        public Task<List<StoryPublishRequest>> GetUserPendingRequestAsync(ApplicationUser user, CancellationToken ct = default);
        public Task CancelRequestAsync(Guid requestId, ApplicationUser user, CancellationToken ct = default);
    }
}
