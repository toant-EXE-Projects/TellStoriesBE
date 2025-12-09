using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;

namespace StoryTeller.Services.Interfaces
{
    public interface IStoryService
    {
        public Task<StoryDTO> CreateAsync(StoryCreateRequest request, ApplicationUser user, StoryMeta? meta = null);
        public Task<StoryDTO> EditAsync(StoryUpdateRequest request, ApplicationUser user, StoryMeta? meta = null);
        public Task<StoryDTO> EditMetaAsync(StoryUpdateMetaRequest metarequest, ApplicationUser user);
        public Task<bool> SoftDelete(Guid id, ApplicationUser user);
        public Task<bool> HardDelete(Guid id);
        public Task<StoryDTO> GetById(Guid id, ApplicationUser? user = null, CancellationToken ct = default);
        public Task<StoryDTO> ReadStory(Guid id, ApplicationUser? user = null, CancellationToken ct = default);
        public Task<PaginatedResult<StoryDTO>> SearchStories(string query, bool? onePanels, bool? isCommunity, int page = 1, int pageSize = 10, CancellationToken ct = default);
        public Task<List<StoryDTO>> GetAll(ApplicationUser? user = null, CancellationToken ct = default);
        public Task<PaginatedResult<StoryDTO>> GetIsFeatured(bool? isCommunity, int page = 1, int pageSize = 10, CancellationToken ct = default);
        public Task<PaginatedResult<StoryDTO>> GetPublished(bool? isCommunity, int page = 1, int pageSize = 10, CancellationToken ct = default);
        //public Task<List<StoryDTO>> GetAll(ApplicationUser user, int skip = 0, int take = 10);
        public Task<List<StoryDTO>> GetAllStoryInType(string storyType);
        public Task<StoryDTO> AddTagsToStoryAsync(AddTagsToStoryRequest request, ApplicationUser user, CancellationToken ct = default);
        public Task<List<StoryDTO>> GetRecentlyPublishedStories(int count = 10);
        public Task<List<StoryDTO>> GetRecentlyPublishedStories(string userId, int count = 10);
        public Task<List<StoryDTO>> GetPublishedStories(string userId);
        public Task<PaginatedResult<StoryPublishRequestDTO>> GetUserRequestPaginatedAsync(ApplicationUser user, int page = 1, int pageSize = 10, CancellationToken ct = default);
        public Task<PaginatedResult<StoryDTO>> GetUserStories(string? query, ApplicationUser user, int page = 1, int pageSize = 10, CancellationToken ct = default);
        public Task<PaginatedResult<StoryPublishRequestDTO>> GetAllPublishRequestPaginatedAsync(int page = 1, int pageSize = 10, CancellationToken ct = default);

        public Task<PaginatedResult<StoryPublishRequestDTO>> GetPendingPublishRequestPaginatedAsync(int page = 1, int pageSize = 10, CancellationToken ct = default);

        public Task<PaginatedResult<StoryPublishRequestDTO>> GetRejectedPublishRequestPaginatedAsync(int page = 1, int pageSize = 10, CancellationToken ct = default);

        public Task<bool> SubmitPublishRequestAsync(StoryPublishCreateRequest request, ApplicationUser user, CancellationToken ct = default);

        public Task<bool> ApprovePublishRequestAsync(StoryPublishReviewRequest request, ApplicationUser user, CancellationToken ct = default);

        public Task<bool> RejectPublishRequestAsync(StoryPublishReviewRequest request, ApplicationUser user, CancellationToken ct = default);

        public Task<bool> CancelPublishRequestAsync(Guid requestId, ApplicationUser user, CancellationToken ct = default);
        
        public Task<PaginatedResult<StoryDTO>> SearchStoriesAndPanels(string? query, bool? onePanels, bool? isCommunity, int page = 1, int pageSize = 10, CancellationToken ct = default);
        public Task<PaginatedResult<StoryDTO>> SearchStoriesByTags(string query, bool? onePanels, bool? isCommunity, int page = 1, int pageSize = 10, CancellationToken ct = default);
        public Task<List<StoryDTO>> GetTopHitStories(string userId, int count = 10);

    }
}
