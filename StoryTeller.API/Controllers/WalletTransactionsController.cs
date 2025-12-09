using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletTransactionsController : ControllerBase
    {
        private readonly IUserWalletTransactionService _userWalletTransactionService;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContext;

        public WalletTransactionsController(
            IMapper mapper,
            IUserWalletTransactionService userWalletTransactionService, 
            IUserContextService userContext
        )
        {
            _userWalletTransactionService = userWalletTransactionService;
            _mapper = mapper;
            _userContext = userContext;
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetTransactions([FromQuery] WalletTransactionRequest query, CancellationToken ct)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var result = await _userWalletTransactionService.GetTransactionsAsync(query, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyTransactions([FromQuery] UserWalletTransactionRequest query, CancellationToken ct)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var request = _mapper.Map<WalletTransactionRequest>(query);
            request.UserQuery = user.Id;

            try
            {
                var result = await _userWalletTransactionService.GetTransactionsAsync(request, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}
