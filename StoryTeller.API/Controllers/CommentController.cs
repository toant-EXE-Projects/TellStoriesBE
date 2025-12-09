using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Models;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IStoryService _storyService;
        private readonly IActivityLogService _activityLogService;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CommentController(
            ICommentService commentService,
            IStoryService storyService,
            IActivityLogService activityLogService,
            IDateTimeProvider dateTimeProvider,
            IMapper mapper,
            IUserContextService userContext)
        {
            _commentService = commentService;
            _storyService = storyService;
            _activityLogService = activityLogService;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
            _userContext = userContext;
        }

        [HttpGet]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(APIResponse<List<CommentDTO>>.SuccessResponse(await _commentService.GetAllAsync()));
        }

        [HttpGet("newest")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetNewestComments(int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            return Ok(APIResponse<List<CommentDTO>>.SuccessResponse(await _commentService.GetNewestAsync(page, pageSize, ct)));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(APIResponse<CommentDTO>.SuccessResponse(await _commentService.GetByIdAsync(new Guid(id))));
        }

        [HttpGet("get-flagged-comment")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetFlaggedComment()
        {
            return Ok(APIResponse<List<CommentDTO>>.SuccessResponse(await _commentService.GetFlaggedComment()));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CommentCreateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var story = await _storyService.GetById(request.StoryId);


                if (request.ReplyTo == null)
                {
                    await _activityLogService.CreateAsync(new ActivityLogCreateRequest
                    {
                        TargetType = ActivityLogConst.TargetType.STORY,
                        Action = ActivityLogConst.Action.COMMENT,
                        Details = $"{_dateTimeProvider.GetSystemCurrentTime().ToString("dd-MM-yyyy")} - {user.DisplayName} (bình luận): {story.Title} - {request.Content}",
                        Category = "Truyện",
                        TargetId = request.StoryId,
                        Reason = "",
                        DeviceInfo = HttpContext.Connection.RemoteIpAddress?.ToString()
                    }, user);
                }
                else
                {
                    var comment = await _commentService.GetByIdAsync((Guid)request.ReplyTo);
                    await _activityLogService.CreateAsync(new ActivityLogCreateRequest
                    {
                        TargetType = ActivityLogConst.TargetType.COMMENT,
                        Action = ActivityLogConst.Action.COMMENT,
                        Details = $"{_dateTimeProvider.GetSystemCurrentTime().ToString("dd-MM-yyyy")} - {user.DisplayName} (bình luận): {story.Title} - {request.Content}",
                        Category = "Truyện",
                        TargetId = request.ReplyTo,
                        Reason = "",
                        DeviceInfo = HttpContext.Connection.RemoteIpAddress?.ToString()
                    }, user);
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }

            try
            {
                return Ok(APIResponse<CommentDTO>.SuccessResponse(await _commentService.CreateAsync(request, user)));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Edit([FromBody] CommentUpdateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var res = await _commentService.EditAsync(request, user);
                return Ok(APIResponse<CommentDTO>.SuccessResponse(res));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403,
                    APIResponse<object>.ErrorResponse(ex.Message)
                );
            }
            
        }

        [HttpPut("meta/{id}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> EditMeta(Guid id, [FromBody] CommentMeta request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var res = await _commentService.EditMetaAsync(id, request, user);
            return Ok(APIResponse<CommentDTO>.SuccessResponse(res));
        }

        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(Guid commentId, CancellationToken ct)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var success = await _commentService.DeleteCommentAsync(commentId, user, ct);
                if (!success)
                    return NotFound(
                        APIResponse<object>.ErrorResponse("Comment not found.")
                    );

                return Ok(APIResponse<object>.SuccessResponse("Comment deleted successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403,
                    APIResponse<object>.ErrorResponse(ex.Message)
                );
            }
            catch (NotFoundException)
            {
                return NotFound("Comment not found.");
            }
        }

        [HttpDelete("hard-delete/{commentId}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> HardDeleteComment(Guid commentId, CancellationToken ct)
        {
            var success = await _commentService.HardDeleteCommentAsync(commentId, ct);
            if (!success)
                return NotFound(
                    APIResponse<object>.ErrorResponse("Comment not found.")
                );

            return Ok(APIResponse<object>.SuccessResponse("Comment permanently deleted."));
        }

        [HttpGet("story/{storyId}")]
        [Authorize]
        public async Task<IActionResult> GetStoryComments(Guid storyId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var res = await _commentService.GetCommentThreadAsync(storyId, page, pageSize, ct);
                return Ok(APIResponse<PaginatedResult<CommentThreadDTO>>.SuccessResponse(res));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("thread/{commentId}")]
        [Authorize]
        public async Task<IActionResult> GetThreadFromComment(Guid commentId, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var thread = await _commentService.GetThreadFromCommentAsync(commentId, ct);
                return Ok(APIResponse<CommentThreadDTO>.SuccessResponse(thread));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("purge-all")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> PurgeAllComments()
        {
            var success = await _commentService.PurgeAllCommentsFromSystemAsync();
            if (success)
                return Ok(APIResponse<object>.SuccessResponse("All comments purged."));

            return StatusCode(500, APIResponse<object>.ErrorResponse("Comments purge failed."));
        }

        [HttpDelete("purge/{storyId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> PurgeStoryComments(Guid storyId)
        {
            var success = await _commentService.PurgeAllCommentsFromStoryAsync(storyId);
            if (success)
                return Ok(APIResponse<object>.SuccessResponse("Story comments purged."));

            return StatusCode(500, APIResponse<object>.ErrorResponse("No comments found for that story."));
        }

        [HttpGet("get-comment-report-heatmap")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetCommentReportHeatmap(Guid storyId)
        {
            return Ok(APIResponse<List<HeatMapReportedCommentResponse>>.SuccessResponse(await _commentService.HeatMapReportedComment(storyId)));
        }
    }
}
