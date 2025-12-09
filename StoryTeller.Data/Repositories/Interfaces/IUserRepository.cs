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
    public interface IUserRepository : IBasicGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<PaginatedResult<ApplicationUser>> SearchAndFilterAsync(string? searchValue, string? filterByRole, string? filterByStatus, string? orderBy, bool asc = true, int pageNumber = 0, int pageSize = 0);
        Task<List<ApplicationUser>> GetActiveUserAsync();
    }
}
