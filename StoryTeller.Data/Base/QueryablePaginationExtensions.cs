using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Base
{
    public static class QueryablePaginationExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
            this IQueryable<T> query,
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default
            )
        {
            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PaginatedResult<T>
            {
                CurrentPage = page,
                PageCount = pageSize,
                TotalItems = totalItems,
                Items = items
            };
        }

        public static PaginatedResult<T> ToPaginatedResult<T>(
            this List<T> query,
            int page = 1,
            int pageSize = 10
            )
        {
            var totalItems = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<T>
            {
                CurrentPage = page,
                PageCount = pageSize,
                TotalItems = totalItems,
                Items = items
            };
        }
    }
}
