using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Data.Models;
using StoryTeller.Data.Repositories.Interfaces;

namespace StoryTeller.Data.Repositories
{
    public class StoryRepository : GenericRepository<Story>, IStoryRepository
    {
        private readonly StoryTellerContext _context;
        //private readonly IGenericRepository<StoryPanel> _panelRepo;

        public StoryRepository(
            StoryTellerContext context,
            IDateTimeProvider dateTimeProvider
          //, IGenericRepository<StoryPanel> panelRepo
        )
            : base(context, dateTimeProvider)
        {
            _context = context;
            //_panelRepo = panelRepo;
        }

        public async Task<Story> AddWithPanelsAsync(Story story, ApplicationUser user)
        {
            await CreateAsync(story, user);

            return story;
        }

        public async Task<Story> EditStoryAsync(Story story, ApplicationUser user)
        {
            await UpdateAsync(story, user);

            return story;
        }

        public IQueryable<Story> GetStoryWithDetailsQuery()
        {
            return _dbSet
                .Include(sp => sp.Panels)
                .Include(auth => auth.CreatedBy)
                .Include(updby => updby.UpdatedBy)
                .Include(tags => tags.StoryTags)
                    .ThenInclude(storyTag => storyTag.Tag)
            ;
        }

        public IQueryable<Story> GetStoryWithMinimumDetailsQuery()
        {
            return _dbSet
                .Include(auth => auth.CreatedBy)
                .Include(updby => updby.UpdatedBy)
                .Include(tags => tags.StoryTags)
                    .ThenInclude(storyTag => storyTag.Tag);
        }

        public IQueryable<Story> SearchStoriesQuery(string query)
        {
            query = query.ToLower();

            return GetStoryWithMinimumDetailsQuery()
                .Where(st =>
                    st.IsPublished &&
                    !st.IsDeleted &&
                    (
                        st.Title.ToLower().Contains(query) ||
                        //st.StoryType.ToString().ToLower().Equals(query) ||
                        (st.Description != null && st.Description.ToLower().Contains(query)) ||
                        (st.Author != null && st.Author.ToLower().Contains(query)) ||
                        (st.ReadingLevel != null && st.ReadingLevel.ToLower().Contains(query)) ||
                        (st.AgeRange != null && st.AgeRange.ToLower().Contains(query)) ||
                        st.StoryTags.Any( tag => 
                            !string.IsNullOrWhiteSpace(tag.Tag.Name) && tag.Tag.Name.ToLower().Contains(query) ||
                            !string.IsNullOrWhiteSpace(tag.Tag.Slug) && tag.Tag.Slug.ToLower().Contains(query)
                        )
                    )
                );
        }

        public async Task<List<Story>> GetAllStoriesWithMinimumDetail()
        {
            return await GetStoryWithMinimumDetailsQuery()
                .ToListAsync();
        }

        public async Task<PaginatedResult<Story>> GetUserCreatedStory(string? query, ApplicationUser user, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            if (!string.IsNullOrWhiteSpace(query))
                query = query.ToLower();

            var baseQuery = GetStoryWithMinimumDetailsQuery()
                .Where(r => r.CreatedBy == user && !r.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query))
            {
                baseQuery = baseQuery.Where(r =>
                    r.Title.ToLower().Contains(query) ||
                    //r.StoryType.ToString().ToLower().Equals(query) ||
                    (r.Description != null && r.Description.ToLower().Contains(query)) ||
                    (r.Author != null && r.Author.ToLower().Contains(query)) ||
                    (r.ReadingLevel != null && r.ReadingLevel.ToLower().Contains(query)) ||
                    (r.AgeRange != null && r.AgeRange.ToLower().Contains(query)) ||
                    r.StoryTags.Any(tag =>
                        !string.IsNullOrWhiteSpace(tag.Tag.Name) && tag.Tag.Name.ToLower().Contains(query) ||
                        !string.IsNullOrWhiteSpace(tag.Tag.Slug) && tag.Tag.Slug.ToLower().Contains(query)
                    )
                );
            }

            return await baseQuery
                .OrderByDescending(r => r.CreatedDate)
                .ToPaginatedResultAsync(page, pageSize, ct);
        }

        public async Task<List<Story>> GetAllStoriesWithMinimumDetail(int skip = 0, int take = 10)
        {
            return await GetStoryWithMinimumDetailsQuery()
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Story>> GetAllStoryInTypeWithDetail(string storyType)
        {
            if (!Enum.TryParse<eStoryType>(storyType, true, out var parsedType))
                return new List<Story>();

            return await GetStoryWithMinimumDetailsQuery()
                .Where( stt => stt.StoryType == parsedType)
                .ToListAsync();
        }

        public async Task<int> IncrementViewCountAsync(Guid storyId, int by = 1, CancellationToken ct = default)
        {
            return await _context.Stories
                .Where(s => s.Id == storyId)
                .ExecuteUpdateAsync(updates =>
                    updates.SetProperty(s => s.ViewCount, s => s.ViewCount + by),
                    ct
                );
        }

        public async Task<Story> GetFullStory(Guid id, CancellationToken ct = default)
        {

            var res = await GetStoryWithDetailsQuery()
                .FirstOrDefaultAsync(st => st.Id == id);

            if (res is null)
                throw new InvalidOperationException("Item not found.");

            return res;
        }

        public async Task<List<Story>> GetRecentlyPublishedStories(int count = 10)
        {
            return await GetStoryWithMinimumDetailsQuery()
                .Where(s => s.IsPublished && s.Status != "Banned")
                .OrderByDescending(s => s.UpdatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Story>> GetRecentlyPublishedStories(string userId, int count = 10)
        {
            return await GetStoryWithMinimumDetailsQuery()
                .Where(s => s.IsPublished && s.Status != "Banned" && s.CreatedBy!.Id == userId)
                .OrderByDescending(s => s.UpdatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Story>> GetPublishedStories(string userId)
        {
            return await GetStoryWithMinimumDetailsQuery()
                .Where(s => s.IsPublished && s.Status != "Banned" && s.CreatedBy!.Id == userId)
                .ToListAsync();
        }

        public IQueryable<Story> SearchStoriesAndPanelsQuery(string? query, bool? onePanels, bool? isCommunity)
        {
            query = query == null ? string.Empty : query.ToLower();
            var stories = GetStoryWithDetailsQuery()
                .Where(st =>
                    st.IsPublished &&
                    (
                        st.Title.ToLower().Contains(query) ||
                        //st.StoryType.ToString().ToLower().Equals(query) ||
                        (!string.IsNullOrWhiteSpace(st.Description) && st.Description.ToLower().Contains(query)) ||
                        (!string.IsNullOrWhiteSpace(st.Author) && st.Author.ToLower().Contains(query)) ||
                        (!string.IsNullOrWhiteSpace(st.ReadingLevel) && st.ReadingLevel.ToLower().Contains(query)) ||
                        (!string.IsNullOrWhiteSpace(st.AgeRange) && st.AgeRange.ToLower().Contains(query)) ||
                        st.StoryTags.Any(tag =>
                            !string.IsNullOrWhiteSpace(tag.Tag.Name) && tag.Tag.Name.ToLower().Contains(query) ||
                            !string.IsNullOrWhiteSpace(tag.Tag.Slug) && tag.Tag.Slug.ToLower().Contains(query)
                        )
                    )
                );

            if (onePanels.HasValue)
            {
                if (onePanels.Value)
                    stories = stories.Where(st => st.Panels.Count == 1);
                else
                    stories = stories.Where(st => st.Panels.Count > 1);
            }

            if (isCommunity.HasValue)
            {
                stories = stories.Where(st => st.IsCommunity == isCommunity.Value);
            }
            return stories;
        }

        public IQueryable<Story> SearchStoriesByTagsQuery(string query, bool? onePanels, bool? isCommunity)
        {
            query = query.ToLower();

            var stories = GetStoryWithMinimumDetailsQuery()
                .Where(st =>
                    st.IsPublished &&
                    st.StoryTags.Any(tag =>
                        !string.IsNullOrWhiteSpace(tag.Tag.Name) &&
                        (
                            tag.Tag.Name.ToLower().Contains(query) ||
                            tag.Tag.Slug.ToLower().Contains(query)
                        )
                    )
                );
            if (onePanels.HasValue)
            {
                if (onePanels.Value)
                    stories = stories.Where(st => st.Panels.Count == 1);
                else
                    stories = stories.Where(st => st.Panels.Count > 1);
            }

            if (isCommunity.HasValue)
            {
                stories = stories.Where(st => st.IsCommunity == isCommunity.Value);
            }
            return stories;
        }

        public async Task<Story> GetStoryWithMinimumDetails(Guid id)
        {

            var res = await _dbSet
                .FirstOrDefaultAsync(st => st.Id == id);

            if (res is null)
                throw new InvalidOperationException("Item not found.");

            return res;
        }

        public async Task<List<Story>> GetTopHitStories(string userId, int count = 10)
        {
            return await GetStoryWithMinimumDetailsQuery()
                .Include(s => s.CreatedBy)
                .Where(s => s.IsPublished && s.Status != "Banned" && s.CreatedBy != null && s.CreatedBy.Id == userId)
                .OrderByDescending(s => s.ViewCount)
                .Take(count)
                .ToListAsync();
        }
    }
}
