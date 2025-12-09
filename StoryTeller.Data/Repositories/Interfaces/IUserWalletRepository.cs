using StoryTeller.Data.Base;
using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories.Interfaces
{
    public interface IUserWalletRepository : IGenericRepository<UserWallet>
    {
        public Task<UserWallet?> GetUserWallet(string userId, CancellationToken ct = default);
    }
}
