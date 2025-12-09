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
    public interface IStoryRepository : IGenericRepository<Story>
    {
        Task<Story> AddWithPanelsAsync(Story story, ApplicationUser user);
        Task<Story> EditStoryAsync(Story story, ApplicationUser user);
        Task<Story> GetFullStory(Guid id, CancellationToken ct = default);
        IQueryable<Story> SearchStoriesQuery(string query);
        IQueryable<Story> GetStoryWithDetailsQuery();
        IQueryable<Story> GetStoryWithMinimumDetailsQuery();
        Task<List<Story>> GetAllStoriesWithMinimumDetail();
        Task<List<Story>> GetAllStoriesWithMinimumDetail(int skip = 0, int take = 10);
        Task<List<Story>> GetAllStoryInTypeWithDetail(string storyType);
        Task<List<Story>> GetRecentlyPublishedStories(int count = 10);
        Task<List<Story>> GetRecentlyPublishedStories(string userId, int count = 10);
        Task<List<Story>> GetPublishedStories(string userId);
        Task<PaginatedResult<Story>> GetUserCreatedStory(string? query, ApplicationUser user, int page = 1, int pageSize = 10, CancellationToken ct = default);
        IQueryable<Story> SearchStoriesAndPanelsQuery(string? query, bool? onePanels, bool? isCommunity);
        IQueryable<Story> SearchStoriesByTagsQuery(string query, bool? onePanels, bool? isCommunity);
        Task<Story> GetStoryWithMinimumDetails(Guid id);
        Task<int> IncrementViewCountAsync(Guid storyId, int by = 1, CancellationToken ct = default);
        Task<List<Story>> GetTopHitStories(string userId, int count = 10);

    }
}
