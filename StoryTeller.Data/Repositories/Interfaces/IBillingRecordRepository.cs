using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories.Interfaces
{
    public interface IBillingRecordRepository : IGenericRepository<BillingRecord>
    {
        public Task<BillingRecord?> GetByBillingId(string billingId, CancellationToken ct);
        public Task<List<BillingRecord>> GetByUserId(string userId);
        public Task<List<BillingRecord>> QueryBillingRecords(string? query, CancellationToken ct);

    }
}
