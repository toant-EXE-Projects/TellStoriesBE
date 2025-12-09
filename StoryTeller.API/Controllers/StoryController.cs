using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoryController : ControllerBase
    {
        private readonly IStoryService _storyService;
        private readonly IActivityLogService _activityLogService;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public StoryController(IStoryService storyService, 
            IDateTimeProvider dateTimeProvider,
            IMapper mapper, 
            IActivityLogService activityLogService, 
            IUserContextService userContext
        )
        {
            _storyService = storyService;
            _mapper = mapper;
            _activityLogService = activityLogService;
            _dateTimeProvider = dateTimeProvider;
            _userContext = userContext;
        }
        private IActionResult? ValidateStoryAccess(StoryDTO story, ApplicationUser user)
        {
            if (story == null)
                return NotFound(
                    APIResponse<object>.ErrorResponse(message: "Story not found")
                );

            if (story.CreatedBy == null)
                return NotFound(
                    APIResponse<object>.ErrorResponse(message: "Story creator not found") // We should never hit this EVER!!! - Duc
                );


            if (story.CreatedBy.Id != user.Id)
                return Unauthorized(
                    APIResponse<object>.ErrorResponse(message: "You are not the owner of this story.")
                );

            return null;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateStory([FromBody] StoryCreateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var storyMeta = new StoryMeta
            {
                IsCommunity = true
            };

            var storyRequest = request;
            var res = await _storyService.CreateAsync(request: storyRequest, user: user, meta: storyMeta);

            return Ok(
                APIResponse<StoryDTO>.SuccessResponse(res, message: "Story created successfully!")
            );
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> EditStory([FromBody] StoryUpdateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var _story = await _storyService.GetById(request.Id);

            var result = ValidateStoryAccess(_story, user);
            if (result != null) return result;

            var storyMeta = new StoryMeta
            {
                IsCommunity = true,
                IsPublished = false
            };

            var storyRequest = request;
            var res = await _storyService.EditAsync(request: storyRequest, user: user, meta: storyMeta);

            return Ok(
                APIResponse<StoryDTO>.SuccessResponse(res, message: "Story edited successfully!")
            );
        }

        [HttpPost("staff/official")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> CreateOfficialStory([FromBody] StoryCreateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var storyMeta = new StoryMeta {
                IsCommunity = false
            };

            var storyRequest = request;
            var res = await _storyService.CreateAsync(request: storyRequest, user: user, meta: storyMeta);

            return Ok(
                APIResponse<StoryDTO>.SuccessResponse(res, message: "Story created successfully!")
            );
        }

        [HttpPut("staff/edit")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> EditStoryStaff([FromBody] StoryUpdateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var storyRequest = request;
            var res = await _storyService.EditAsync(request: storyRequest, user: user);

            return Ok(
                APIResponse<StoryDTO>.SuccessResponse(res, message: "Story edited successfully!")
            );
        }

        [HttpPatch("EditStoryMeta")]
        //[HttpPatch("meta")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> EditStoryMeta([FromBody] StoryUpdateMetaRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var res = await _storyService.EditMetaAsync(metarequest: request, user: user);

            return Ok(
                APIResponse<StoryDTO>.SuccessResponse(res, message: "Story edited successfully!")
            );
        }

        [HttpDelete("{storyId}")]
        [Authorize]
        public async Task<IActionResult> SoftDeleteStory(Guid storyId)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var _story = await _storyService.GetById(storyId);

            var result = ValidateStoryAccess(_story, user);
            if (result != null) return result;

            var res = await _storyService.SoftDelete(storyId, user);

            return Ok(
                APIResponse<StoryDTO>.SuccessResponse(res, message: "Story deleted successfully!")
            );
        }

        [HttpDelete("staff/soft-delete/{storyId}")]
        [Authorize]
        public async Task<IActionResult> SoftDeleteStoryStaff(Guid storyId)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var res = await _storyService.SoftDelete(storyId, user);

            return Ok(
                APIResponse<StoryDTO>.SuccessResponse(res, message: "Story deleted successfully!")
            );
        }

        [HttpDelete("staff/hard-delete/{storyId}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> StaffHardDeleteStory(Guid storyId)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var res = await _storyService.HardDelete(storyId);

            return Ok(
                APIResponse<StoryDTO>.SuccessResponse(res, message: "Story deleted successfully!")
            );
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var story = await _storyService.GetById(id, user, ct);
            if (story == null) return NotFound(
                    APIResponse<object>.ErrorResponse(message: "Story not found")
                  );
            //await _activityLogService.CreateAsync(new ActivityLogCreateRequest
            //{
            //    TargetType = "Story",
            //    Action = "View",
            //    Details = $"{DateTime.Now.Date.ToString("dd-MM-yyyy")} - {user.DisplayName}(view): {story.Title}",
            //    Category = "Story",
            //    TargetId = story.Id,
            //    Reason = "",
            //    DeviceInfo = HttpContext.Connection.RemoteIpAddress?.ToString()
            //}, user);

            return Ok(
                APIResponse<StoryDTO>.SuccessResponse(story)
            );
        }

        [HttpGet("read/{id}")]
        [Authorize]
        public async Task<IActionResult> ReadStory(Guid id, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var story = await _storyService.ReadStory(id, user, ct);
                if (story == null) return NotFound(
                    APIResponse<object>.ErrorResponse(message: "Story not found")
                );

                await _activityLogService.CreateAsync(new ActivityLogCreateRequest
                {
                    TargetType = ActivityLogConst.TargetType.STORY,
                    Action = ActivityLogConst.Action.VIEW,
                    Details = $"{_dateTimeProvider.GetSystemCurrentTime().ToString("dd-MM-yyyy")} - {user.DisplayName} (xem truyện): {story.Title}",
                    Category = "Truyện",
                    TargetId = id,
                    Reason = "",
                    DeviceInfo = HttpContext.Connection.RemoteIpAddress?.ToString()
                }, user);

                return Ok(
                    APIResponse<StoryDTO>.SuccessResponse(story)
                );
            }
            catch (Exception ex)
            {
                return NotFound(
                    APIResponse<object>.ErrorResponse(message: ex.Message)
                );
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var objs = await _storyService.GetAll(user, ct);
            if (objs == null) return NotFound();

            return Ok(
                APIResponse<List<StoryDTO>>.SuccessResponse(objs)
            );
        }

        [HttpGet("published")]
        public async Task<IActionResult> GetAllPublished([FromQuery] bool? isCommunity = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var objs = await _storyService.GetPublished(isCommunity, page, pageSize, ct);
            if (objs == null) return NotFound();

            return Ok(
                APIResponse<PaginatedResult<StoryDTO>>.SuccessResponse(objs)
            );
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetAllFeatured([FromQuery] bool? isCommunity = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var objs = await _storyService.GetIsFeatured(isCommunity, page, pageSize, ct);
            if (objs == null) return NotFound();

            return Ok(
                APIResponse<PaginatedResult<StoryDTO>>.SuccessResponse(objs)
            );
        }
        /// <summary>
        /// Search for PUBLISHED stories by keyword.
        /// </summary>
        /// <remarks>
        /// Search if query contains keywords in these metadata fields:
        /// - Title
        /// - Description
        /// - Author
        /// - Story Type
        /// - Reading Level
        /// - Age Range
        /// - Tag Name
        /// <br/>
        /// And filter story by panels:
        /// - Stories with 1 panel if onePanel is true 
        /// - Stories with multiple panels if onePanel is false 
        /// - Both type of Stories if onePanel is null
        /// </remarks>
        /// <param name="query">The search keyword</param>
        /// <param name="onePanel">The filter stories with one or multiple panels keyword</param>
        /// <param name="isCommunity">Filter for Community Story.</param>
        /// <param name="page">The page number of the result set (default 1).</param>
        /// <param name="pageSize">The number of results per page (default 10).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// A paginated list of story results that match the query.
        /// Returns 404 if no results are found.
        /// </returns>
        /// <response code="200">Returns the list of matched stories</response>
        /// <response code="404">No matching stories found</response>
        /// <response code="401">Unauthorized access</response>
        [HttpGet("search/{query}")]
        //[Authorize]
        public async Task<IActionResult> SearchStories(string query, [FromQuery] bool? onePanel = null, [FromQuery] bool? isCommunity = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var objs = await _storyService.SearchStories(query, onePanel, isCommunity, page, pageSize, ct);
            if (objs == null) return NotFound();

            return Ok(
                APIResponse<PaginatedResult<StoryDTO>>.SuccessResponse(objs)
            );
        }

        [HttpGet("search/all")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> SearchStoriesAndPanels([FromQuery] string? query, [FromQuery] bool? onePanel = null, [FromQuery] bool? isCommunity = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var objs = await _storyService.SearchStoriesAndPanels(query, onePanel, isCommunity, page, pageSize, ct);
            if (objs == null) return NotFound();

            return Ok(
                APIResponse<PaginatedResult<StoryDTO>>.SuccessResponse(objs)
            );
        }

        [HttpGet("search/tag/{query}")]
        //[Authorize]
        public async Task<IActionResult> SearchStoriesByTags(string query, [FromQuery] bool? onePanel = null, [FromQuery] bool? isCommunity = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var objs = await _storyService.SearchStoriesByTags(query, onePanel, isCommunity, page, pageSize, ct);
            if (objs == null) return NotFound();

            return Ok(
                APIResponse<PaginatedResult<StoryDTO>>.SuccessResponse(objs)
            );
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyStories([FromQuery] string? query = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var objs = await _storyService.GetUserStories(query, user, page, pageSize, ct);
            if (objs == null) return NotFound();

            return Ok(
                APIResponse<PaginatedResult<StoryDTO>>.SuccessResponse(objs)
            );
        }

        [HttpGet("GetAllStoryInType")]
        //[HttpGet("type/{storyType}")]
        //[Authorize]
        public async Task<IActionResult> GetAllStoryInType(string storyType)
        {
            var objs = await _storyService.GetAllStoryInType(storyType);
            if (objs == null) return NotFound();

            return Ok(
                APIResponse<List<StoryDTO>>.SuccessResponse(objs)
            );
        }

        [HttpPost("AddTags")]
        //[HttpPost("tags")]
        [Authorize]
        public async Task<IActionResult> AddTags([FromBody] AddTagsToStoryRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var _story = await _storyService.GetById(request.StoryId);

            var result = ValidateStoryAccess(_story, user);
            if (result != null) return result;

            var res = await _storyService.AddTagsToStoryAsync(request, user);

            return Ok(
                APIResponse<StoryDTO>.SuccessResponse(res)
            );
        }

        [HttpGet("recently-published")]
        [Authorize]
        public async Task<IActionResult> GetRecentlyPublishedStories([FromQuery] int count = 10)
        {
            var objs = await _storyService.GetRecentlyPublishedStories(count);
            if (objs == null) return NotFound();

            return Ok(
                APIResponse<List<StoryDTO>>.SuccessResponse(objs)
            );
        }

        [HttpGet("recently-published/user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetRecentlyPublishedStories(string userId, [FromQuery] int count = 10)
        {
            var objs = await _storyService.GetRecentlyPublishedStories(userId, count);
            if (objs == null) return NotFound();

            return Ok(
                APIResponse<List<StoryDTO>>.SuccessResponse(objs)
            );
        }

        [HttpGet("published/user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetPublishedStoriesByUserId(string userId)
        {
            var objs = await _storyService.GetPublishedStories(userId);
            if (objs == null) return NotFound();

            return Ok(
                APIResponse<List<StoryDTO>>.SuccessResponse(objs)
            );
        }

        [HttpGet("publish-request/me")]
        [Authorize]
        public async Task<IActionResult> GetAllUserRequest([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var result = await _storyService.GetUserRequestPaginatedAsync(user, page, pageSize, ct);
            return Ok(APIResponse<PaginatedResult<StoryPublishRequestDTO>>.SuccessResponse(result));
        }

        [HttpGet("publish-request/all")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetAllRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _storyService.GetAllPublishRequestPaginatedAsync(page, pageSize, ct);
            return Ok(APIResponse<PaginatedResult<StoryPublishRequestDTO>>.SuccessResponse(result));
        }

        [HttpGet("publish-request/pending")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetPendingRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _storyService.GetPendingPublishRequestPaginatedAsync(page, pageSize, ct);
            return Ok(APIResponse<PaginatedResult<StoryPublishRequestDTO>>.SuccessResponse(result));
        }

        [HttpGet("publish-request/rejected")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetRejectedRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _storyService.GetRejectedPublishRequestPaginatedAsync(page, pageSize, ct);
            return Ok(APIResponse<PaginatedResult<StoryPublishRequestDTO>>.SuccessResponse(result));
        }

        [HttpPost("publish-request/submit")]
        [Authorize]
        public async Task<IActionResult> SubmitRequest([FromBody] StoryPublishCreateRequest request, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var success = await _storyService.SubmitPublishRequestAsync(request, user, ct);
                return Ok(APIResponse<object>.SuccessResponse(success, "Publish request submitted."));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("publish-request/approve")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> ApproveRequest([FromBody] StoryPublishReviewRequest request, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try {
                var success = await _storyService.ApprovePublishRequestAsync(request, user, ct);
                return Ok(APIResponse<object>.SuccessResponse(success, "Publish request approved."));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }

        }

        [HttpPut("publish-request/reject")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> RejectRequest([FromBody] StoryPublishReviewRequest request, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var success = await _storyService.RejectPublishRequestAsync(request, user, ct);
                return Ok(APIResponse<object>.SuccessResponse(success, "Publish request rejected."));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("publish-request/cancel/{requestId:guid}")]
        [Authorize]
        public async Task<IActionResult> CancelRequest([FromRoute] Guid requestId, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var success = await _storyService.CancelPublishRequestAsync(requestId, user, ct);
                return Ok(APIResponse<object>.SuccessResponse(success, "Publish request cancelled."));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("top-hit/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetTopHitStories([FromRoute] string userId, [FromQuery] int count = 10)
        {
            try
            {
                var objs = await _storyService.GetTopHitStories(userId, count);
                if (objs == null) return NotFound();

                return Ok(
                    APIResponse<List<StoryDTO>>.SuccessResponse(objs)
                );
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }

        }
    }
}
