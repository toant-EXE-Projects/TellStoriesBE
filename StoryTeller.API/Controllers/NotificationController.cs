using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IUserContextService _userContext;
        private readonly IMapper _mapper;

        public NotificationController(INotificationService notificationService, IUserContextService userContext, IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
            _userContext = userContext;
        }

        /// <summary>
        /// Gets the user's notification
        /// </summary>
        /// <param name="searchQuery">Search query containing notification's: Title, Message, Sender, Type</param>
        /// <param name="onlyUnread">Only query unread notifications</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetUserNotifications(
            [FromQuery] string? searchQuery = null,
            [FromQuery] bool onlyUnread = false,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10, 
            CancellationToken ct = default
        )
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var result = await _notificationService.GetUserNotificationsPagedAsync(user.Id, searchQuery, onlyUnread, page, pageSize, ct);
                return Ok(APIResponse<PaginatedResult<NotificationDTO>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Returns Notification and marks it as read
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("me/{id}")]
        [Authorize]
        public async Task<IActionResult> ReadNotification(Guid id, CancellationToken ct)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));
            try
            {
                var notif = await _notificationService.ReadNotification(id, user, ct);
                return Ok(APIResponse<NotificationDTO>.SuccessResponse(notif));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Mark Notification as read
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("me/mark-as-read/{id}")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var success = await _notificationService.MarkAsRead(id, user, ct);
                return Ok(APIResponse<object>.SuccessResponse(success, "Notification marked as read."));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.SuccessResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("me/mark-as-read-all")]
        [Authorize]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var success = await _notificationService.MarkAllAsRead(user, ct);

            try
            {
                if (!success)
                    return Ok(APIResponse<object>.SuccessResponse(null, "No unread notifications."));

                return Ok(APIResponse<object>.SuccessResponse(null, "Marked all notification as read."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Send Notification
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("send")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> SendNotification(NotificationSendRequest request, CancellationToken ct)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));
            try
            {
                var success = await _notificationService.NotifyAsync(request, user, ct);
                return Ok(APIResponse<object>.SuccessResponse(success, "Notification sent."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Get current authenticated user sent Notifications.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("sent")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetMySentNotifications(
            [FromQuery] string? searchQuery = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default
        )
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var res = await _notificationService.GetMySentNotificationsAsync(user, searchQuery, page, pageSize, ct);
                return Ok(APIResponse<PaginatedResult<NotificationDTO>>.SuccessResponse(res));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}
