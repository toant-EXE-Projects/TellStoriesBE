using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Base
{
    public interface IGenericRepository<T>
    {
        Task<List<T>> GetAllAsync(CancellationToken ct = default);

        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<T?> GetByIdAsync(string id, CancellationToken ct = default);

        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);

        Task CreateAsync(T entity, ApplicationUser? user, CancellationToken ct = default);
        Task CreateRangeAsync(List<T> entities, ApplicationUser user, CancellationToken ct = default);
        Task UpdateAsync(T entity, ApplicationUser? user, CancellationToken ct = default);

        Task SoftRemove(T entity, ApplicationUser user, CancellationToken ct = default);
        void RemoveRange(IEnumerable<T> entities);

        Task SoftRemoveRange(IEnumerable<T> entities, ApplicationUser user, CancellationToken ct = default);

        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        void Remove(T entity);
    }
}
