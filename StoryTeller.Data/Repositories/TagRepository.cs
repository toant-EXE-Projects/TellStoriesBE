using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Repositories.Interfaces;
using StoryTeller.Data.Utils;

namespace StoryTeller.Data.Repositories
{
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        private readonly StoryTellerContext _context;

        public TagRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.Name.Trim().ToLower().Equals(name.Trim().ToLower()));
        }
        public async Task<Tag?> GetBySlugAsync(string slug)
        {
            return await _dbSet
                .FirstOrDefaultAsync(t => t.Slug.Trim().ToLower().Equals(slug.Trim().ToLower()));
        }
    }
}
