using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using System.Linq.Expressions;

namespace StoryTeller.Data.Base
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        //protected StoryTellerContext _context;
        protected DbSet<T> _dbSet;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GenericRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        {
            //_context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
            _dateTimeProvider = dateTimeProvider ?? throw new InvalidOperationException("DateTimeProvider is required.");
        }

        public async Task<List<T>> GetAllAsync(CancellationToken ct = default) => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) => await _dbSet.FindAsync(id);

        public async Task<T?> GetByIdAsync(string id, CancellationToken ct = default) => await _dbSet.FindAsync(id);

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) => await _dbSet.FindAsync(id);

        public async Task CreateAsync(T entity, ApplicationUser? user, CancellationToken ct = default)
        {
            entity.CreatedDate = _dateTimeProvider.GetSystemCurrentTime();
            entity.UpdatedAt = _dateTimeProvider.GetSystemCurrentTime();
            if (user != null)
            {
                entity.CreatedBy = user;
                entity.UpdatedBy = user;
            }
            await _dbSet.AddAsync(entity);
        }

        public async Task CreateRangeAsync(List<T> entities, ApplicationUser user, CancellationToken ct = default)
        {
            foreach (var entity in entities)
            {
                entity.CreatedDate = _dateTimeProvider.GetSystemCurrentTime();
                entity.CreatedBy = user;
                entity.UpdatedAt = _dateTimeProvider.GetSystemCurrentTime();
                entity.UpdatedBy = user;
            }
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task UpdateAsync(T entity, ApplicationUser? user, CancellationToken ct = default)
        {
            entity.UpdatedAt = _dateTimeProvider.GetSystemCurrentTime();
            if (user != null)
            {
                entity.UpdatedBy = user;
            }
            _dbSet.Update(entity);
        }

        public void UpdateRange(List<T> entities, ApplicationUser user, CancellationToken ct = default)
        {
            foreach (var entity in entities)
            {
                entity.CreatedDate = _dateTimeProvider.GetSystemCurrentTime();
                entity.CreatedBy = user;
            }
            _dbSet.UpdateRange(entities);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task SoftRemoveRange(IEnumerable<T> entities, ApplicationUser user, CancellationToken ct = default)
        {
            foreach (var entity in entities)
            {
                entity.DeletionDate = _dateTimeProvider.GetSystemCurrentTime();
                entity.DeleteBy = user;
                entity.IsDeleted = true;
                _dbSet.Update(entity);
            }
        }
        public async Task SoftRemove(T entity, ApplicationUser user, CancellationToken ct = default)
        {
            entity.DeletionDate = _dateTimeProvider.GetSystemCurrentTime();
            entity.DeleteBy = user;
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(ct);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return await _dbSet.AnyAsync(predicate, ct);
        }
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate, ct);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
