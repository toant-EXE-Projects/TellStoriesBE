using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Models;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using System.Security.Claims;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContext;

        public UserController(IUserService userService, IMapper mapper, IUserContextService userContext)
        {
            _userService = userService;
            _mapper = mapper;
            _userContext = userContext;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return NotFound(APIResponse<object>.ErrorResponse(message: "User not found"));

            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
                return NotFound(APIResponse<object>.ErrorResponse(message: "User not found."));
            
            return Ok(APIResponse<UserDTO>.SuccessResponse(data: user, message: "Profile Fetched Successfully"));
        }

        [HttpPatch("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] ProfileUpdateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var updatedData = _mapper.Map<UserUpdateRequest>(request);
            try
            {
                var isUpdated = await _userService.UpdateAsync(user.Id, updatedData, user);
                if (isUpdated)
                    return Ok(APIResponse<object>.SuccessResponse(isUpdated, message: "Profile updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(message: ex.Message));
            }

            return NotFound(APIResponse<object>.ErrorResponse(message: "User not found"));
        }

        [HttpPatch("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound(APIResponse<object>.ErrorResponse(message: "User not found"));

            if (!ModelState.IsValid)
                return BadRequest("Invalid input.");

            var result = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(APIResponse<string>.ErrorResponse(errors: errors, message: "Password change failed."));
            }

            return Ok(APIResponse<object>.SuccessResponse(message: "Password changed successfully."));
        }

        [HttpGet]
        //[Authorize(Roles = $"{Roles.Admin}, {Roles.Moderator}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(APIResponse<List<UserDTO>>.SuccessResponse(users));
        }

        [HttpGet("by-id/{id}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(APIResponse<object>.ErrorResponse(message: "User not found."));

            return Ok(APIResponse<UserDTO>.SuccessResponse(user));
        }

        [HttpGet("by-email/{email}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            return user == null
                ? NotFound(APIResponse<UserDTO>.ErrorResponse(message: "User not found"))
                : Ok(APIResponse<UserDTO>.SuccessResponse(user));
        }

        [HttpPost("create")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] UserCreateRequest request)
        {
            try
            {
                var user = await _userService.CreateAsync(request);
                return user == null
                    ? NotFound(APIResponse<UserDTO>.ErrorResponse(message: "Internal error when create user!"))
                    : Ok(APIResponse<UserDTO>.SuccessResponse(user, message: "user created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(message: ex.Message));
            }

        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var updUser = await _userService.UpdateAsync(id, request, user);
                return updUser
                    ? Ok(APIResponse<UserDTO>.SuccessResponse(updUser, message: "User updated successfully"))
                    : NotFound(APIResponse<UserDTO>.ErrorResponse(message: "User not found"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(message: ex.Message));
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
                var delUser = await _userService.DeleteAsync(id, user);
                return delUser
                    ? Ok(APIResponse<UserDTO>.SuccessResponse(delUser, message: "User deleted successfully"))
                    : NotFound(APIResponse<UserDTO>.ErrorResponse(message: "User not found"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(message: ex.Message));
            }
        }

        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> DeleteOwnAccount([FromBody] DeleteAccountRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var success = await _userService.DeleteOwnAccountAsync(user.Id, request.CurrentPassword, user);
                if (!success)
                    return NotFound();

                return Ok(APIResponse<object>.SuccessResponse("Your account has been deleted"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("search")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> SearchUser(string searchValue)
        {
            var users = await _userService.SearchAsync(searchValue);
            return Ok(APIResponse<List<UserDTO>>.SuccessResponse(users));
        }

        [HttpPut("change-role/{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ChangeRole(string id, string role)
        {
            try
            {
                var result = await _userService.ChangeRoleAsync(id, role);
                return result
                    ? Ok(APIResponse<UserDTO>.SuccessResponse(result, message: "Change role successfully"))
                    : NotFound(APIResponse<UserDTO>.ErrorResponse(message: "User not found"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }

        }

        [HttpGet("dashboard")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> DashBoard(int statisticPeriod = 90)
        {
            var result = await _userService.Dashboard(statisticPeriod);
            return Ok(APIResponse<DashboardResponse>.SuccessResponse(result));
        }

        [HttpGet("query-user")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> QueryUser([FromQuery]UserQueryRequest request)
        {
            try
            {
                var result = await _userService.QueryUser(request);
                return Ok(APIResponse<DashboardResponse>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }

        }

        [HttpGet("get-subscription-detail/{userId}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetSubscriptionDetail(string userId)
        {
            try
            {
                var result = await _userService.GetSubscriptionDetail(userId);
                return Ok(APIResponse<SubscriptionDetailResponse>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("billing-history/me")]
        [Authorize]
        public async Task<IActionResult> BillingHistory(bool? ascOrder, int page = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not found");

            try { 
                var result = await _userService.BillingHistory(userId, ascOrder, page, pageSize);
                return Ok(APIResponse<BillingHistoryResponse>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("billing-history/{userId}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> BillingHistory(string userId, bool? ascOrder, int page = 1, int pageSize = 10)
        {
            try
            {
                var result = await _userService.BillingHistory(userId, ascOrder, page, pageSize);
                return Ok(APIResponse<BillingHistoryResponse>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Search for all billing history by keyword.
        /// </summary>
        /// <remarks>
        /// Search if query contains keywords match billing history :
        /// - Subscription's name
        /// - User's name
        /// - User's email
        /// - Payment method
        /// - Status
        /// <br/>
        /// And ordered billing history by Paid date:
        /// - ascending order if ascOrder is true 
        /// - descending order if ascOrder is false 
        /// - default order if ascOrder is null
        /// </remarks>
        /// <param name="query">The search keyword</param>
        /// <param name="ascOrder">The keyword order of return data</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>
        /// A list of billing history that match the query.
        /// </returns>
        /// <response code="200">Returns the list of matched stories</response>
        /// <response code="401">Unauthorized access</response>
        [HttpGet("billing-history-query")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> BillingHistoryQuery(string? query, bool? ascOrder, int page = 1, int pageSize = 10)
        {
            var result = await _userService.BillingHistoryQuery(query, ascOrder, page, pageSize);
            return Ok(APIResponse<PaginatedResult<BillingRecordDTO>>.SuccessResponse(result));
        }

        [HttpGet("revenue-detail")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> BillingHistoryQuery([FromQuery]int period = 30)
        {
            var result = await _userService.RevenueDetail(period);
            return Ok(APIResponse<List<BillingRecordDTO>>.SuccessResponse(result));
        }
    }
}
