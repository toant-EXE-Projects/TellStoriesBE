using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Data.Models;
using StoryTeller.Data.Repositories.Interfaces;

namespace StoryTeller.Data.Repositories
{
    public class UserWalletTransactionRepository : GenericRepository<UserWalletTransaction>, IUserWalletTransactionRepository
    {
        private readonly StoryTellerContext _context;

        public UserWalletTransactionRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
        }

        public async Task<PaginatedResult<UserWalletTransaction>> GetAllTransactions(
            int page = 1,
            int pageSize = 10,
            string? userQuery = null,
            WalletTransactionType? type = null,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken ct = default)
        {
            var query = _dbSet
                .Include(t => t.Wallet)
                .ThenInclude(w => w.User)
                .Include(t => t.PerformedByUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(userQuery))
            {
                userQuery.Trim();
                query = query.Where(t =>
                    t.Wallet.UserId == userQuery ||
                    t.Wallet.User.Email == userQuery
                );
            }


            if (type.HasValue)
                query = query.Where(t => t.Type == type);

            if (from.HasValue)
                query = query.Where(t => t.CreatedDate >= from.Value);

            if (to.HasValue)
                query = query.Where(t => t.CreatedDate <= to.Value);

            return await query
                .OrderByDescending(t => t.CreatedDate)
                .ToPaginatedResultAsync(page, pageSize, ct);
        }


    }
}
