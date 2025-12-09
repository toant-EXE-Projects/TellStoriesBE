using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.API.Utils;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserWalletController : ControllerBase
    {
        private readonly IUserWalletService _userWalletService;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContext;
        private readonly Validator _validator;

        public UserWalletController(IMapper mapper,
            IUserWalletService userWalletService, IUserContextService userContext, Validator validator)
        {
            _userWalletService = userWalletService;
            _mapper = mapper;
            _userContext = userContext;
            _validator = validator;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyWallet(CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var wallet = await _userWalletService.GetOrCreateUserWalletDTO(user.Id, user, ct);
                return Ok(APIResponse<object>.SuccessResponse(wallet, "Wallet fetched successfully"));
            }
            catch (NotFoundException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("add/{userId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AddBalance(string userId, WalletDeltaRequest request, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));
            try
            {
                var wallet = await _userWalletService.AddBalanceAsync(userId, request.Amount, user, request.Reason, ct: ct);
                return Ok(APIResponse<UserWalletDTO>.SuccessResponse(wallet));
            }
            catch (NotFoundException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }

        }

        [HttpPost("subtract/{userId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SubtractBalance(string userId, WalletDeltaRequest request, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));
            try
            {
                var wallet = await _userWalletService.SubtractBalanceAsync(userId, request.Amount, user, request.Reason, ct: ct);
                return Ok(APIResponse<UserWalletDTO>.SuccessResponse(wallet));
            }
            catch (NotFoundException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("set/{userId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SetBalance(string userId, WalletDeltaRequest request, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));
            try
            {
                var wallet = await _userWalletService.SetBalanceAsync(userId, request.Amount, user, request.Reason, ct: ct);
                return Ok(APIResponse<UserWalletDTO>.SuccessResponse(wallet));
            }
            catch (NotFoundException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("toggle-lock/{userId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ToggleWalletLockAsync(string userId, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var wallet = await _userWalletService.ToggleWalletLockAsync(userId, user, ct);
                return Ok(APIResponse<UserWalletDTO>.SuccessResponse(wallet));
            }
            catch (NotFoundException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("lock/{userId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> LockWallet(string userId, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var wallet = await _userWalletService.LockWalletAsync(userId, user, ct);
                return Ok(APIResponse<UserWalletDTO>.SuccessResponse(wallet));
            }
            catch (NotFoundException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("unlock/{userId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UnlockWallet(string userId, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var wallet = await _userWalletService.UnlockWalletAsync(userId, user, ct);
                return Ok(APIResponse<UserWalletDTO>.SuccessResponse(wallet));
            }
            catch (NotFoundException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}
