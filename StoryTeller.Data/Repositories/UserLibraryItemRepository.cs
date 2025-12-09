using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Repositories.Interfaces;

namespace StoryTeller.Data.Repositories
{
    public class UserLibraryItemRepository : GenericRepository<UserLibraryItem>, IUserLibraryItemRepository
    {
        private readonly StoryTellerContext _context;

        public UserLibraryItemRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
        {
            _context = context;
        }
    }
}
