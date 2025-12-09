using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.DBContext;

namespace StoryTeller.Data.Base
{
    public class BasicGenericRepository<T> : IBasicGenericRepository<T> where T : class
    {
        //protected StoryTellerContext _context;
        protected DbSet<T> _dbSet;

        public BasicGenericRepository(StoryTellerContext context)
        {
            //_context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public async Task<T?> GetByIdAsync(string id) => await _dbSet.FindAsync(id);

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task CreateAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
