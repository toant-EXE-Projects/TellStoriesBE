using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Enums;
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
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContext;


        public SubscriptionController(ISubscriptionService subscriptionService, 
            IMapper mapper, 
            IUserContextService userContext
        )
        {
            _subscriptionService = subscriptionService;
            _mapper = mapper;
            _userContext = userContext;
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(APIResponse<List<SubscriptionDTO>>.SuccessResponse(await _subscriptionService.GetAllByAdminAsync()));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                return Ok(APIResponse<SubscriptionDTO>.SuccessResponse(await _subscriptionService.GetByIdAsync(new Guid(id))));

            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Creates a new subscription.
        /// </summary>
        /// <param name="request">
        /// The subscription creation payload containing:
        /// - <description><c>Name</c> – Subscription display name.</description>
        /// - <description><c>Price</c> – The price of the subscription.</description>
        /// - <description><c>Type</c> – Subscription type.</description>
        /// - <description><c>DurationDays</c> (optional) – Number of days the subscription is valid.</description>
        /// - <description><c>PointsCost</c> (optional) – Points required to redeem this subscription.</description>
        /// - <description><c>PurchaseMethod</c> (optional) – One of: <c>MoneyOnly[0]</c>, <c>PointsOnly[1]</c>, <c>MoneyOrPoints[2]</c>.</description>
        /// - <description><c>IsActive</c> (optional) – Indicates if the subscription is available for purchase.</description>
        /// </param>
        /// <returns>
        /// Returns a <see cref="SubscriptionDTO"/> the newly created subscription.
        /// </returns>
        [HttpPost("create")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(SubscriptionCreateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            return Ok(APIResponse<SubscriptionDTO>.SuccessResponse(await _subscriptionService.CreateAsync(request, user), "Create subscription successful!"));
        }

        [HttpPut("update")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(SubscriptionUpdateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));
            try
            {
                return Ok(APIResponse<SubscriptionDTO>.SuccessResponse(await _subscriptionService.UpdateAsync(request, user), "Update subscription successful!"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
            
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                return Ok(APIResponse<SubscriptionDTO>.SuccessResponse(await _subscriptionService.SoftDelete(new Guid(id), user), "Delete subscription successful!"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }

        }

        [HttpGet("dashboard")]
        [Authorize]
        public async Task<IActionResult> SubscriptionDashboard()
        {
            return Ok(APIResponse<SubscriptionDashboardResponse>.SuccessResponse(await _subscriptionService.SubscriptionDashboard()));
        }

        [HttpGet("active")]
        [Authorize]
        public async Task<IActionResult> GetAllActive([FromQuery] SubscriptionPurchaseMethod? method = null , CancellationToken ct = default)
        {
            return Ok(APIResponse<List<SubscriptionDTO>>.SuccessResponse(await _subscriptionService.GetAllActiveAsync(method, ct)));
        }

        [HttpPost("redeem/{subscriptionId}")]
        [Authorize]
        public async Task<IActionResult> RedeemSubscriptionWithPoints(Guid subscriptionId, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));
            try
            {

                await _subscriptionService.RedeemWithPointsAsync(subscriptionId, user, ct);

                return Ok(APIResponse<object>.SuccessResponse("Subscription redeemed with points."));
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

        [HttpPost("give-subscription")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SubscribeAsync(string userId, Guid subscriptionId, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                await _subscriptionService.SubscribeOrExtendAsync(subscriptionId, user, ct);
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
            return Ok(APIResponse<object>.SuccessResponse($"Gave User:{userId}, Subcription: {subscriptionId}"));
        }

        [HttpGet("expiring-soon")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetExpiringSubscriptions([FromQuery] int days = 3, CancellationToken ct = default)
        {
            var subs = await _subscriptionService.UpcomingExpirationsAsync(days, ct);

            return Ok(APIResponse<object>.SuccessResponse(subs));
        }

        [HttpGet("email-expiring-soon")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> EmailExpiringSubscriptions([FromQuery] int days = 3, CancellationToken ct = default)
        {
            var emailsSent = await _subscriptionService.NotifyUpcomingExpirationsAsync(days, ct);

            return Ok(APIResponse<object>.SuccessResponse(emailsSent));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> CheckStatusAsync(CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var userSub = await _subscriptionService.GetUserActiveSubscriptionDTOAsync(user.Id, ct);
            return Ok(APIResponse<object>.SuccessResponse(userSub));
        }

        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> CancelSubscription(CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var res = await _subscriptionService.CancelUserSubscription(user, ct);
                if (res)
                {
                    return Ok(
                        APIResponse<object>.SuccessResponse(
                            res,
                            "Disable subscription successful!"
                        )
                    );
                }
                else
                {
                    return BadRequest(
                        APIResponse<object>.ErrorResponse(
                            "User does not have an active subscription."
                        )
                    );
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("refresh-all-status")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RefreshAllSubscriptionStatus(CancellationToken ct = default)
        {
            var updatedCount = await _subscriptionService.MarkExpiredSubscriptionsAsync(ct);

            if (updatedCount == 0)
                return Ok(
                    APIResponse<object>.SuccessResponse("No subscriptions were updated.")
                );

            return Ok(
                APIResponse<object>.SuccessResponse($"Updated {updatedCount} subscriptions.")
            );
        }

        [HttpGet("dashboard-get-subscribers")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SubscriptionDashboardGetSubscribers(int page = 1, int pageSize = 10)
        {
            return Ok(APIResponse<PaginatedResult<SubscriptionDetailResponse>>.SuccessResponse(await _subscriptionService.SubscriptionDashboardGetSubscribers(page, pageSize)));
        }

        [HttpGet("dashboard-get-new-subscribers")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SubscriptionDashboardGetNewSubscribers(int page = 1, int pageSize = 10)
        {
            return Ok(APIResponse<PaginatedResult<SubscriptionDetailResponse>>.SuccessResponse(await _subscriptionService.SubscriptionDashboardGetNewSubscribers(page, pageSize)));
        }

        [HttpGet("dashboard-get-quit-subscribers")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SubscriptionDashboardGetQuitSubscribers(int page = 1, int pageSize = 10)
        {
            return Ok(APIResponse<PaginatedResult<SubscriptionDetailResponse>>.SuccessResponse(await _subscriptionService.SubscriptionDashboardGetQuitSubscribers(page, pageSize)));
        }

        [HttpGet("dashboard-get-subscribers-by-subscription")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SubscriptionDashboardGetSubscribersBySubscription(int page = 1, int pageSize = 10)
        {
            return Ok(APIResponse<List<PaginatedResult<SubscriptionDetailResponse>>>.SuccessResponse(await _subscriptionService.SubscriptionDashboardGetSubscribersBySubscription(page, pageSize)));
        }

        [HttpGet("dashboard-get-recent-subscribers-within-period")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SubscriptionDashboardGetRecentSubscribers(int period = 0, int page = 1, int pageSize = 10)
        {
            return Ok(APIResponse<PaginatedResult<Subscriber>>.SuccessResponse(await _subscriptionService.SubscriptionDashboardGetRecentSubscribers(period, page, pageSize)));
        }

        /// <summary>
        /// Search for PUBLISHED stories by keyword.
        /// </summary>
        /// <remarks>
        /// Search if query contains keywords in these fields:
        /// - User.DisplayName
        /// - Subscription.Name
        /// <br/>
        /// </remarks>
        /// <param name="query">The search keyword</param>
        /// <returns>
        /// A list of subscribers that match the query.
        /// Returns empty list if no results are found.
        /// </returns>
        /// <response code="200">Returns the list of matched stories</response>
        /// <response code="401">Unauthorized access</response>
        [HttpGet("query")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> QuerySubscribers(string query, int page = 1, int pageSize = 10)
        {
            return Ok(APIResponse<PaginatedResult<SubscriptionDetailResponse>>.SuccessResponse(await _subscriptionService.QuerySubscribers(query, page, pageSize)));
        }
    }
}
