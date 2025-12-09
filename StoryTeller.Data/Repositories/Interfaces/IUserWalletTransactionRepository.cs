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
    public interface IUserWalletTransactionRepository : IGenericRepository<UserWalletTransaction>
    {
        public Task<PaginatedResult<UserWalletTransaction>> GetAllTransactions(
            int page = 1,
            int pageSize = 10,
            string? userQuery = null,
            WalletTransactionType? type = null,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken ct = default);
    }
}
