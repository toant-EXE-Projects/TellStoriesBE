using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using System.Security.Claims;

namespace StoryTeller.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityLogController : ControllerBase
    {
        private readonly IActivityLogService _activityLogService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ActivityLogController(IActivityLogService activityLogService, IMapper mapper, UserManager<ApplicationUser> userManager, IDateTimeProvider dateTimeProvider)
        {
            _activityLogService = activityLogService;
            _mapper = mapper;
            _userManager = userManager;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(APIResponse<List<ActivityLogDTO>>.SuccessResponse(await _activityLogService.GetAllAsync()));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(APIResponse<ActivityLogDTO>.SuccessResponse(await _activityLogService.GetByIdAsync(new Guid(id))));
        }

        /// <summary>
        /// Get log to track user activity.
        /// </summary>
        /// <remarks>
        /// Get all activity log from a user and can filter them by Date from and Date 
        /// <br/>
        /// - if not null, the API will return all user activity logs with created date from getByDateFrom to getByDateTo
        /// - if getByDateFrom is null, the API will return all user activity logs from the first log created to activity logs with created date getByDateTo
        /// - if getByDateTo is null, the API will return all user activity logs with created date from getByDateFrom to current time
        /// - if both is null, the API will just return all user activity logs
        /// - if you want to get logs from a specific date, just set the getByDateFrom and getByDateTo to be a same date
        /// <br/>
        /// </remarks>
        /// <returns>
        /// <param name="id">The user ID</param>
        /// <param name="getByDateFrom">The start date we will get activity logs</param>
        /// <param name="getByDateTo">The end date where we will stop getting later's logs</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="ct"></param>
        /// </returns>
        /// <response code="200">Get logs successsfully</response>
        /// <response code="401">Unauthorized access</response>
        [HttpGet("user-id/{id}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetByUserId([FromRoute] string id, [FromQuery] DateTime? getByDateFrom, [FromQuery] DateTime? getByDateTo, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Unauthorized("User not found!");
            try
            {
                return Ok(APIResponse<List<ActivityLogDTO>>.SuccessResponse(await _activityLogService.GetByUserIdAsync(id, getByDateFrom, getByDateTo, page, pageSize, ct)));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Create log to track user activity.
        /// </summary>
        /// <remarks>
        /// Create a new activity log object contain these fields and save to database:
        /// - TargetType (Truyện, Bình Luận hoặc Người Dùng)
        /// - Action (Hệ Thống, Xuất Bản Truyện, Bình Luận, Đánh Giá hoặc Xem Truyện)
        /// - Timestamp: thời điểm tạo log
        /// - Details: miêu tả chi tiết hoạt động của người dùng (dd-MM-yyyy - "tên người dùng" (Action): "hoạt động của người dùng" - thêm nội dung của hành động(nếu có))
        /// - Category: Loại của log? tui để nó bằng TargetType thôi
        /// - TargetId" Id của target
        /// - Reason: Nguyên nhân của hành động (dùng khi người dùng report)
        /// - DeviceInfo
        /// <br/>
        /// Ví dụ Detail: 12-08-2025 - Toant (bình luận): Câu truyện nghe để chìm và giấc ngủ - Rất thích hợp để nghe khi mất ngủ
        /// </remarks>
        /// <returns>
        /// A paginated list of story results that match the query.
        /// Returns 404 if no results are found.
        /// </returns>
        /// <response code="200">Create log successsfully</response>
        /// <response code="404">No matching target found</response>
        /// <response code="401">Unauthorized access</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateActivityLog([FromBody] ActivityLogCreateRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null)
                return Unauthorized("User not found");

            return Ok(APIResponse<List<ActivityLogDTO>>.SuccessResponse(await _activityLogService.CreateAsync(request, user)));
        }

        [HttpDelete]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ClearLog()
        {
            var result = await _activityLogService.ClearLogAsync();
            return result
                ? Ok(APIResponse<List<ActivityLogDTO>>.SuccessResponse("Activity Logs Cleared"))
                : Ok(APIResponse<List<ActivityLogDTO>>.ErrorResponse("Something went wrong when clearing logs in database"));
        }

        [HttpPost("log-logout")]
        [Authorize]
        public async Task<IActionResult> CreateLogLogout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null)
                return Unauthorized("User not found");

            var log = new ActivityLogCreateRequest
            {
                TargetType = ActivityLogConst.TargetType.USER,
                Action = ActivityLogConst.Action.SYSTEM,
                Details = $"{_dateTimeProvider.GetSystemCurrentTime().ToString("dd-MM-yyyy")} - {user.DisplayName} (hệ thống): Người dùng đã đăng xuất",
                Category = "Hệ thống",
                TargetId = new Guid(user.Id),
                Reason = "",
                DeviceInfo = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            return Ok(APIResponse<List<ActivityLogDTO>>.SuccessResponse(await _activityLogService.CreateAsync(log, user)));
        }
    }
}
