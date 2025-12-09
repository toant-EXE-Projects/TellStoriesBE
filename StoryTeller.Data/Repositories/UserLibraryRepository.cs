using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Repositories.Interfaces;

namespace StoryTeller.Data.Repositories
{
    public class UserLibraryRepository : GenericRepository<UserLibrary>, IUserLibraryRepository
    {
        private readonly StoryTellerContext _context;

        public UserLibraryRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
        }
        public async Task<List<UserLibrary>> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(ul => ul.LibraryItems
                    .Where(i => !i.IsDeleted && i.Story != null && !i.Story.IsDeleted))
                    .ThenInclude(i => i.Story)
                .Where(ul => 
                    !ul.IsDeleted &&
                    ul.UserId == userId
                )
                .ToListAsync();
        }
        public async Task<UserLibrary?> GetCollectionAsync(Guid libraryId)
        {
            return await _context.UserLibraries
                .Include(lib => lib.LibraryItems.Where(item => !item.Story.IsDeleted))
                    .ThenInclude(item => item.Story)
                        .ThenInclude(story => story.StoryTags)

                //.Include(lib => lib.LibraryItems)
                //    .ThenInclude(item => item.Story)
                //        .ThenInclude(story => story.Panels)
                .FirstOrDefaultAsync(lib => lib.Id == libraryId);
        }
    }
}
