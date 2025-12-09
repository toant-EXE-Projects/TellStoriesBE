using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Repositories.Interfaces;

namespace StoryTeller.Data.Repositories
{
    public class UserWalletRepository : GenericRepository<UserWallet>, IUserWalletRepository
    {
        private readonly StoryTellerContext _context;

        public UserWalletRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
        }

        public async Task<UserWallet?> GetUserWallet(string userId, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(x => x.User)
                .Where(w => w.UserId.Equals(userId))
                .FirstOrDefaultAsync(ct);
        }
    }
}
