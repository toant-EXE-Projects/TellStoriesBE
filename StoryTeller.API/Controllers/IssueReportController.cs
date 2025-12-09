using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueReportController : ControllerBase
    {
        private readonly IIssueReportService _IssueReportService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserContextService _userContext;
        private readonly IActivityLogService _activityLogService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public IssueReportController(IIssueReportService issueReportService, IMapper mapper, UserManager<ApplicationUser> userManager, IUserContextService userContext, IActivityLogService activityLogService, IDateTimeProvider dateTimeProvider)
        {
            _IssueReportService = issueReportService;
            _mapper = mapper;
            _userManager = userManager;
            _userContext = userContext;
            _activityLogService = activityLogService;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet("staff/get-all")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(APIResponse<PaginatedResult<IssueReportDTO>>.SuccessResponse(await _IssueReportService.GetAllAsPaginatedResultAsync(page, pageSize)));
        }

        [HttpGet("staff/get-by-id/{id}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(APIResponse<IssueReportDTO>.SuccessResponse(await _IssueReportService.GetByIdAsync(new Guid(id))));
        }

        [HttpGet("staff/get-by-issuer-id/{id}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetByUserId(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            return Ok(APIResponse<List<IssueReportDTO>>.SuccessResponse(await _IssueReportService.GetByUserIdAsync(id)));
        }

        [HttpGet("staff/get-reported-issue/{id}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetReportedIssueByUserId(string id)
        {
            return Ok(APIResponse<List<IssueReportDTO>>.SuccessResponse(await _IssueReportService.GetReportedIssueByUserId(id)));
        }

        /// <summary>
        /// Send a issue to report a user or problem.
        /// </summary>
        /// <remarks>
        /// Create a issue report by enter the following fields:
        /// - IssueType -> Spam, Harassment or Toxic Behavior (optional)
        /// - TargetType -> Comment, Story, Bug or Other
        /// - TargetId -> Id of the story or comment that is reported
        /// - Attachment -> The file contain evident for the report (optional)
        /// - Description -> Description of report (optional)
        /// </remarks>
        /// <response code="200">IssueReport send successfully</response>
        /// <response code="404">Story or comment for report not found</response>
        /// <response code="401">Unauthorized access</response>
        [HttpPost("user-issue")]
        [Authorize]
        public async Task<IActionResult> CreateIssueReport([FromBody] IssueReportCreateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var result = await _IssueReportService.CreateAsync(request, user);

            var logTargetType = IssueReportConst.TargetType.ToActivityLogConst(request.TargetType);
            if (ActivityLogConst.IsTargetType(logTargetType))
            {
                await _activityLogService.CreateAsync(new ActivityLogCreateRequest
                {
                    TargetType = logTargetType,
                    Action = ActivityLogConst.Action.REPORT,
                    Details = $"{_dateTimeProvider.GetSystemCurrentTime().ToString("dd-MM-yyyy")} - {user.DisplayName} (báo cáo): {result.Description}",
                    Category = logTargetType,
                    TargetId = request.TargetId,
                    Reason = request.Description,
                    DeviceInfo = HttpContext.Connection.RemoteIpAddress?.ToString()
                }, user);
            }

            return Ok(APIResponse<IssueReportDTO>.SuccessResponse(result, "Issue sent successfully"));
        }

        [HttpPut("staff/resolved/{id}")]
        [Authorize]
        public async Task<IActionResult> IssueReportResolved(Guid id)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            return Ok(APIResponse<IssueReportDTO>.SuccessResponse(await _IssueReportService.ResolveAsync(id, user), "Issue status updated successfully!"));
        }

        [HttpDelete("staff/delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteIssueReport(Guid id)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            return Ok(APIResponse<IssueReportDTO>.SuccessResponse(await _IssueReportService.DeleteAsync(id, user), "Issue deleted successfully!"));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetByMyUserId()
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return NotFound("User not found");

            return Ok(APIResponse<List<IssueReportDTO>>.SuccessResponse(await _IssueReportService.GetByUserIdAsync(user.Id)));
        }
    }
}
