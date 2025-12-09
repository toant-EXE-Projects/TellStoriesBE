using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Data.Logger;
using StoryTeller.Data.Models;
using StoryTeller.Data.Utils;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;

namespace StoryTeller.Services.Implementations
{
    public class StoryService : IStoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerService _logger;
        private readonly ISystemConfigurationService _systemConfigService;

        public StoryService(
            IUnitOfWork uow, 
            IConfiguration config, 
            IMapper mapper, 
            IDateTimeProvider dateTimeProvider, 
            UserManager<ApplicationUser> userManager,
            ILoggerService loggerService,
            ISystemConfigurationService systemConfigService
        )
        {
            _uow = uow;
            _config = config;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
            _userManager = userManager;
            _logger = loggerService;
            _systemConfigService = systemConfigService;
        }

        public async Task<StoryDTO> CreateAsync(StoryCreateRequest request, ApplicationUser user, StoryMeta? meta = null)
        {
            var story = _mapper.Map<Story>(request);

            if (meta != null)
                _mapper.Map(meta, story);

            foreach (var panel in story.Panels)
            {
                panel.CreatedDate = _dateTimeProvider.GetSystemCurrentTime();
                panel.CreatedBy = user;
            }

            if (SystemConst.StoryType == 1)
            {
                story.StoryType = story.Panels != null &&
                    story.Panels.Count > 1
                    ? eStoryType.MultiPanel
                    : eStoryType.SinglePanel;
            }
            else if (SystemConst.StoryType == 2)
            {
                bool hasImage = story.Panels != null
                    && story.Panels.Any(
                        panel => !string.IsNullOrWhiteSpace(panel.ImageUrl)
                    );

                story.StoryType = hasImage
                    ? eStoryType.Illustrated
                    : eStoryType.Narrative;
            }

            var repo = _uow.Stories;

            var savedStory = await repo.AddWithPanelsAsync(story, user);
            await _uow.SaveChangesAsync();

            if (request.Tags != null)
            {
                if (request.Tags.TagNames.Count > 0)
                {
                    var tags = _mapper.Map<AddTagsToStoryRequest>(request.Tags);
                    tags.StoryId = savedStory.Id;

                    await AddTagsToStoryAsync(tags, user);
                }
            }

            await _uow.SaveChangesAsync();

            return _mapper.Map<StoryDTO>(savedStory);
        }

        public async Task<StoryDTO> EditAsync(StoryUpdateRequest request, ApplicationUser user, StoryMeta? meta = null)
        {
            var _oldStory = await _uow.Stories.GetFullStory(request.Id);
            _oldStory.Panels.Clear(); // clear panels then map the new data, CreatedDate is wiped but eh can't be bothered

            _mapper.Map(request, _oldStory);

            if (meta != null)
                _mapper.Map(meta, _oldStory);

            if (request.Panels != null && request.Panels.Count > 0)
            {
                foreach (var panel in _oldStory.Panels)
                {
                    panel.CreatedDate = _dateTimeProvider.GetSystemCurrentTime();
                    panel.CreatedBy = user;
                    panel.UpdatedAt = _dateTimeProvider.GetSystemCurrentTime();
                    panel.UpdatedBy = user;
                }
            }

            _oldStory.StoryType = _oldStory.Panels != null && 
                _oldStory.Panels.Count > 1
                    ? eStoryType.MultiPanel
                    : eStoryType.SinglePanel;

            if (SystemConst.StoryType == 1)
            {
                _oldStory.StoryType = _oldStory.Panels != null &&
                    _oldStory.Panels.Count > 1
                    ? eStoryType.MultiPanel
                    : eStoryType.SinglePanel;
            }
            else if (SystemConst.StoryType == 2)
            {
                bool hasImage = _oldStory.Panels != null
                    && _oldStory.Panels.Any(
                        panel => !string.IsNullOrWhiteSpace(panel.ImageUrl)
                    );

                _oldStory.StoryType = hasImage
                    ? eStoryType.Illustrated
                    : eStoryType.Narrative;
            }

            if (request.Tags != null && request.Tags.TagNames.Count > 0)
            {
                var tags = _mapper.Map<AddTagsToStoryRequest>(request.Tags);
                tags.StoryId = _oldStory.Id;

                await AddTagsToStoryAsync(tags, user);
            }

            var savedStory = await _uow.Stories.EditStoryAsync(_oldStory, user);

            await _uow.SaveChangesAsync();

            return _mapper.Map<StoryDTO>(savedStory);
        }

        public async Task<StoryDTO> EditMetaAsync(StoryUpdateMetaRequest metaRequest, ApplicationUser user)
        {
            var story = await _uow.Stories.GetFullStory(metaRequest.Id);
            if (story == null)
                throw new NotFoundException($"Story with ID {metaRequest.Id} not found.");

            if (metaRequest != null)
                _mapper.Map(metaRequest.Meta, story);

            foreach (var panel in story.Panels)
            {
                panel.UpdatedAt = _dateTimeProvider.GetSystemCurrentTime();
                panel.UpdatedBy = user;
            }

            var repo = _uow.Stories;
            var savedStory = await repo.EditStoryAsync(story, user);

            await _uow.SaveChangesAsync();

            return _mapper.Map<StoryDTO>(savedStory);
        }

        public async Task<bool> SoftDelete(Guid id, ApplicationUser user)
        {
            var story = await _uow.Stories.GetFullStory(id);
            if (story == null)
                throw new NotFoundException($"Story with ID {id} not found.");

            await _uow.Stories.SoftRemove(story, user);

            await _uow.SaveChangesAsync();

            return true;
        }

        public async Task<bool> HardDelete(Guid id)
        {
            var story = await _uow.Stories.GetFullStory(id);
            if (story == null)
                throw new NotFoundException($"Story with ID {id} not found.");

            _uow.Stories.Remove(story);

            await _uow.SaveChangesAsync();

            return true;
        }

        public async Task<StoryDTO> GetById(Guid id, ApplicationUser? user = null, CancellationToken ct = default)
        {
            var query = _uow.Stories.GetStoryWithDetailsQuery();

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var isStaff = roles.Intersect(Roles.Staff).Any();
                if (!isStaff)
                    query = query.Where(s => !s.IsDeleted);
            }
            var obj = await query
                .FirstOrDefaultAsync(st => st.Id == id);
            return _mapper.Map<StoryDTO>(obj);
        }

        public async Task<bool> IncrementViews(Guid storyId, CancellationToken ct = default)
        {
            var result = await _uow.Stories.IncrementViewCountAsync(storyId, 1, ct);
            return result > 0;
        }

        public async Task<StoryDTO> ReadStory(Guid id, ApplicationUser? user = null, CancellationToken ct = default)
        {
            var story = await GetById(id, user, ct);
            
            if (
                story != null &&
                story.IsPublished == true &&
                story.CreatedBy!.Id != user!.Id
            )
            {
                var incView = await IncrementViews(id, ct);
                if (!incView)
                    _logger.LogWarning($"Could not increment view for story: {story.Id} | {story.Title}");
            }

            return story;
        }

        public async Task<PaginatedResult<StoryDTO>> GetIsFeatured(bool? isCommunity, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var query = _uow.Stories.GetStoryWithMinimumDetailsQuery();

            var objs = query
                .Where(s => s.IsFeatured && s.IsPublished);

            if (isCommunity.HasValue)
            {
                objs = objs.Where(s => s.IsCommunity == isCommunity);
            }

            var res = await objs.ToPaginatedResultAsync(page, pageSize, ct);

            return _mapper.Map<PaginatedResult<StoryDTO>>(res);
        }

        public async Task<PaginatedResult<StoryDTO>> GetPublished(bool? isCommunity, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var query = _uow.Stories.GetStoryWithMinimumDetailsQuery();

            var objs = query
                .Where(s => s.IsPublished);

            if (isCommunity.HasValue)
            {
                objs = objs.Where(s => s.IsCommunity == isCommunity);
            }

            var res = await objs.ToPaginatedResultAsync(page, pageSize, ct);

            return _mapper.Map<PaginatedResult<StoryDTO>>(res);
        }

        public async Task<List<StoryDTO>> GetAll(ApplicationUser? user = null, CancellationToken ct = default)
        {
            var query = _uow.Stories.GetStoryWithMinimumDetailsQuery();


            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var isStaff = roles.Intersect(Roles.Staff).Any();
                if (!isStaff)
                    query = query.Where(s => !s.IsDeleted);
            }
            //query.OrderByDescending(st => st.CreatedDate);

            var objs = await query.ToListAsync();
            return _mapper.Map<List<StoryDTO>>(objs);
        }

        //public async Task<List<StoryDTO>> GetAll(ApplicationUser user, int skip = 0, int take = 10)
        //{
        //    var objs = await _uow.Stories.GetAllStoriesWithMinimumDetail(skip, take);
        //    return _mapper.Map<List<StoryDTO>>(objs);
        //}
        public async Task<PaginatedResult<StoryDTO>> GetUserStories(string? query, ApplicationUser user, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var objs = await _uow.Stories.GetUserCreatedStory(query, user, page, pageSize, ct);
            return _mapper.Map<PaginatedResult<StoryDTO>>(objs);
        }
        public async Task<List<StoryDTO>> GetAllStoryInType(string storyType)
        {
            var objs = await _uow.Stories.GetAllStoryInTypeWithDetail(storyType);
            return _mapper.Map<List<StoryDTO>>(objs);
        }

        public async Task<PaginatedResult<StoryDTO>> SearchStories(string query, bool? onePanels, bool? isCommunity, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var stories = _uow.Stories.SearchStoriesQuery(query);

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
            var objs = await stories.ToPaginatedResultAsync(page, pageSize, ct);
            return _mapper.Map<PaginatedResult<StoryDTO>>(objs);
        }

        public async Task<bool> RemoveStoryTagsAsync(Story story, ApplicationUser user, CancellationToken ct = default)
        {
            var oldStoryTags = story.StoryTags.Where(st => st.StoryId == story.Id).ToList();
            if (oldStoryTags.Any())
            {
                _uow.StoryTags.RemoveRange(oldStoryTags);
                await _uow.SaveChangesAsync(ct);
            }
            return true;
        }

        public async Task<StoryDTO> AddTagsToStoryAsync(AddTagsToStoryRequest request, ApplicationUser user, CancellationToken ct = default)
        {
            var story = await _uow.Stories.GetByIdAsync(request.StoryId, ct);
            if (story == null) throw new NotFoundException("Story not found");

            await RemoveStoryTagsAsync(story, user, ct);

            var storyTags = new List<StoryTag>();

            foreach (var tagName in request.TagNames)
            {
                var tag = await _uow.Tags.GetByNameAsync(tagName);
                if (tag == null)
                {
                    tag = new Tag
                    {
                        Name = tagName,
                        Slug = tagName.ToSlug()
                    };
                    await _uow.Tags.CreateAsync(tag, user, ct);
                }

                storyTags.Add(new StoryTag
                {
                    StoryId = story.Id,
                    TagId = tag.Id,
                    User = user,
                    CreatedBy = user,
                    CreatedDate = _dateTimeProvider.GetSystemCurrentTime()
                });
            }

            await _uow.StoryTags.CreateRangeAsync(storyTags, user, ct);

            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<StoryDTO>(story);
        }

        public async Task<List<StoryDTO>> GetRecentlyPublishedStories(int count = 10)
        {
            var result = await _uow.Stories.GetRecentlyPublishedStories(count);
            return _mapper.Map<List<StoryDTO>>(result);
        }

        public async Task<List<StoryDTO>> GetRecentlyPublishedStories(string userId, int count = 10)
        {
            var result = await _uow.Stories.GetRecentlyPublishedStories(userId, count);
            return _mapper.Map<List<StoryDTO>>(result);
        }

        public async Task<List<StoryDTO>> GetPublishedStories(string userId)
        {
            var result = await _uow.Stories.GetPublishedStories(userId);
            return _mapper.Map<List<StoryDTO>>(result);
        }

        public async Task<PaginatedResult<StoryPublishRequestDTO>> GetUserRequestPaginatedAsync(ApplicationUser user, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _uow.StoryPublishRequests.GetUserRequestPaginatedAsync(user, page, pageSize, ct);
            return _mapper.Map<PaginatedResult<StoryPublishRequestDTO>>(result);
        }

        public async Task<PaginatedResult<StoryPublishRequestDTO>> GetAllPublishRequestPaginatedAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _uow.StoryPublishRequests.GetAllPaginatedAsync(page, pageSize, ct);
            return _mapper.Map<PaginatedResult<StoryPublishRequestDTO>>(result);
        }

        public async Task<PaginatedResult<StoryPublishRequestDTO>> GetPendingPublishRequestPaginatedAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _uow.StoryPublishRequests.GetPendingPaginatedAsync(page, pageSize, ct);
            return _mapper.Map<PaginatedResult<StoryPublishRequestDTO>>(result);
        }

        public async Task<PaginatedResult<StoryPublishRequestDTO>> GetRejectedPublishRequestPaginatedAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _uow.StoryPublishRequests.GetRejectedPaginatedAsync(page, pageSize, ct);
            return _mapper.Map<PaginatedResult<StoryPublishRequestDTO>>(result);
        }

        public async Task<bool> SubmitPublishRequestAsync(StoryPublishCreateRequest request, ApplicationUser user, CancellationToken ct = default)
        {
            var story = await _uow.Stories.GetFullStory(request.StoryId);
            if (story == null)
                throw new NotFoundException("Story not found");
            if (story.CreatedBy != user)
                throw new UnauthorizedAccessException("You are not the owner of this story.");

            var pendingRequests = await _uow.StoryPublishRequests.GetUserPendingRequestAsync(user);

            var sysConfig_requestLimit = await _systemConfigService.GetIntAsync(
                SystemConst.Keys.StoryPublish_MaxPendingRequests_Default, 
                SystemConst.Values.StoryPublish_MaxPendingRequests_Default,
                ct
            );
            var activeSub = await _uow.UserSubscriptions.GetUserActiveSubscription(user.Id, ct);
            if (activeSub != null)
            {
                sysConfig_requestLimit = await _systemConfigService.GetIntAsync(
                    SystemConst.Keys.StoryPublish_MaxPendingRequests_Tier1,
                    SystemConst.Values.StoryPublish_MaxPendingRequests_Tier1,
                    ct
                );
            }

            if (pendingRequests.Count > sysConfig_requestLimit)
                throw new InvalidOperationException("Publish request limit reached.");

            var publishRequest = _mapper.Map<StoryPublishRequest>(request);
            publishRequest.Status = StoryPublishRequestStatus.Pending;

            await _uow.StoryPublishRequests.CreateAsync(publishRequest, user, ct);

            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApprovePublishRequestAsync(StoryPublishReviewRequest request, ApplicationUser user, CancellationToken ct = default)
        {
            var publishRequest = await _uow.StoryPublishRequests.GetByIdAsync(request.Id);
            if (publishRequest == null)
                throw new NotFoundException("Story publish request not found");
            
            var story = await _uow.Stories.GetFullStory(publishRequest.StoryId);
            if (story == null)
                throw new NotFoundException("Story not found");

            if (publishRequest.Status != StoryPublishRequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be reviewed.");

            _mapper.Map(request, publishRequest);
            publishRequest.Status = StoryPublishRequestStatus.Approved;
            publishRequest.ReviewedBy = user;
            publishRequest.ReviewedAt = _dateTimeProvider.GetSystemCurrentTime();

            story.IsPublished = true;
            await _uow.StoryPublishRequests.UpdateAsync(publishRequest, user, ct);

            //Log lại để biết là truyện đã được publish
            await _uow.ActivityLogs.CreateAsync(new ActivityLog
            {
                UserId = story.CreatedBy!.Id,
                TargetType = ActivityLogConst.TargetType.STORY,
                Action = ActivityLogConst.Action.PUBLISH,
                Timestamp = _dateTimeProvider.GetSystemCurrentTime(),
                Details = $"{_dateTimeProvider.GetSystemCurrentTime().ToString("dd-MM-yyyy")} - {user.DisplayName} (xuất bản truyện): {story.Title}",
                Category = "Truyện",
                TargetId = request.Id,
                Reason = "",
                ActorRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                DeviceInfo = ""
            }, user);

            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectPublishRequestAsync(StoryPublishReviewRequest request, ApplicationUser user, CancellationToken ct = default)
        {
            var publishRequest = await _uow.StoryPublishRequests.GetByIdAsync(request.Id);


            if (publishRequest == null)
                throw new NotFoundException("Story publish request not found");

            var story = await _uow.Stories.GetFullStory(publishRequest.StoryId);
            if (story == null)
                throw new NotFoundException("Story not found");

            if (publishRequest.Status != StoryPublishRequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be reviewed.");

            _mapper.Map(request, publishRequest);
            publishRequest.Status = StoryPublishRequestStatus.Rejected;
            publishRequest.ReviewedBy = user;
            publishRequest.ReviewedAt = _dateTimeProvider.GetSystemCurrentTime();

            await _uow.StoryPublishRequests.UpdateAsync(publishRequest, user, ct);

            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelPublishRequestAsync(Guid requestId, ApplicationUser user, CancellationToken ct = default)
        {
            var request = await _uow.StoryPublishRequests.GetByIdAsync(requestId);
            if (request == null)
                throw new NotFoundException("Couldn't find story publish request.");

            if (request.CreatedBy != user)
                throw new UnauthorizedAccessException("You are not the owner of this request.");

            var story = await _uow.Stories.GetFullStory(request.StoryId);
            if (story == null)
                throw new NotFoundException("Story not found");
            if (story.CreatedBy != user)
                throw new UnauthorizedAccessException("You are not the owner of this story.");

            await _uow.StoryPublishRequests.CancelRequestAsync(requestId, user, ct);

            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<StoryDTO>> SearchStoriesAndPanels(string? query, bool? onePanels, bool? isCommunity, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var objs = await _uow.Stories.SearchStoriesAndPanelsQuery(query, onePanels, isCommunity)
                .ToPaginatedResultAsync(page, pageSize, ct);
            return _mapper.Map<PaginatedResult<StoryDTO>>(objs);
        }
        public async Task<PaginatedResult<StoryDTO>> SearchStoriesByTags(string query, bool? onePanels, bool? isCommunity, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var objs = await _uow.Stories.SearchStoriesByTagsQuery(query, onePanels, isCommunity)
                .ToPaginatedResultAsync(page, pageSize, ct);
            return _mapper.Map<PaginatedResult<StoryDTO>>(objs);
        }

        public async Task<List<StoryDTO>> GetTopHitStories(string userId, int count = 10)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            var result = await _uow.Stories.GetTopHitStories(userId, count);
            return _mapper.Map<List<StoryDTO>>(result);
        }
    }
}
