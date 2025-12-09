using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
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
    public class UserRepository : BasicGenericRepository<ApplicationUser>, IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly StoryTellerContext _context;

        public UserRepository(UserManager<ApplicationUser> userManager, StoryTellerContext context) : base(context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<PaginatedResult<ApplicationUser>> SearchAndFilterAsync(
            string? searchValue,
            string? filterByRole,
            string? filterByStatus,
            string? orderBy,
            bool asc = true,
            int pageNumber = 0,
            int pageSize = 0
            )
        {
            IQueryable<ApplicationUser> query = _dbSet.AsQueryable();

            if (searchValue != null) { query = Search(searchValue, query); }
            if (filterByRole != null) { query = FilterByRole(filterByRole, query); }
            if (filterByStatus != null) { query = FilterByStatus(filterByStatus, query); }
            if (orderBy != null) { query = Order(orderBy, asc, query); }
            if (pageNumber > 0 && pageSize > 0)
            {
                return await QueryablePaginationExtensions.ToPaginatedResultAsync(query, pageNumber, pageSize);
            }
            else
            {
                return await QueryablePaginationExtensions.ToPaginatedResultAsync(query);
            }
        }

        private IQueryable<ApplicationUser> Search(string searchValue, IQueryable<ApplicationUser> query)
        {
            return query.Where(q => q.DisplayName != null && q.DisplayName.ToLower().Contains(searchValue.ToLower()) || q.Email != null && q.Email.ToLower().Contains(searchValue.ToLower()));
        }

        private IQueryable<ApplicationUser> FilterByRole(string filterByRole, IQueryable<ApplicationUser> query)
        {
            return query.Where(q => q.UserType != null && filterByRole.ToLower().Contains(q.UserType.ToLower()));
        }

        private IQueryable<ApplicationUser> FilterByStatus(string filterByStatus, IQueryable<ApplicationUser> query)
        {
            return query.Where(q => q.Status != null && filterByStatus.ToLower().Contains(q.Status.ToLower()));
        }

        private IQueryable<ApplicationUser> Order(string orderBy, bool asc, IQueryable<ApplicationUser> query)
        {
            switch (orderBy.ToLower())
            {
                case "email":
                    query = asc
                        ? query.OrderBy(q => q.Email)
                        : query.OrderByDescending(q => q.Email);
                    break;

                case "displayname":
                    query = asc
                        ? query.OrderBy(q => q.DisplayName)
                        : query.OrderByDescending(q => q.DisplayName);
                    break;

                case "role":
                    query = asc
                        ? query.OrderBy(q => q.UserType)
                        : query.OrderByDescending(q => q.UserType);
                    break;

                case "status":
                    query = asc
                        ? query.OrderBy(q => q.Status)
                        : query.OrderByDescending(q => q.Status);
                    break;
                default:
                    break;
            }

            return query;
        }

        public async Task<List<ApplicationUser>> GetActiveUserAsync()
        {
            return await _dbSet
                .Where(u => !(bool)u.IsDeleted!)
                .ToListAsync();
        }
    }
}
