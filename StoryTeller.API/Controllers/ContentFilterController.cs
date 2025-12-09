using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Utils;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentFilterController : ControllerBase
    {
        private readonly ICensorService _censorService;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContext;

        public ContentFilterController(ICensorService censorService, IMapper mapper, IUserContextService userContext)
        {
            _censorService = censorService;
            _mapper = mapper;
            _userContext = userContext;
        }

        [HttpPost("filter")]
        [Authorize]
        public async Task<IActionResult> FilterContentAsync(FilterContentRequest request, CancellationToken ct = default)
        {
            try
            {
                var result = await _censorService.FilterContentAsync(request.Text, request.BlockIfProfanity, ct);

                return Ok(APIResponse<string>.SuccessResponse(result));
            }
            catch (CensoredException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(
                    message: "Text contains inappropriate content.",
                    errors: ex.MatchedWords
                ));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("words")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> AddWordsAsync([FromBody] List<string> request, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            await _censorService.AddCensoredWordsAsync(request, user, ct);
            return Ok(APIResponse<object>.SuccessResponse("Words added successfully"));
        }

        [HttpDelete("words")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> DeleteWordsAsync([FromBody] List<Guid> wordIds, CancellationToken ct = default)
        {
            var success = await _censorService.DeleteCensoredWordsAsync(wordIds, ct);
            return success
                ? Ok(APIResponse<object>.SuccessResponse("Words deleted successfully"))
                : NotFound(APIResponse<object>.ErrorResponse("No matching words found."));
        }

        [HttpGet("words")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> GetWordsAsync(CancellationToken ct = default)
        {
            var words = await _censorService.GetCensoredWordsListAsync(ct);
            return Ok(APIResponse<object>.SuccessResponse(words));
        }
    }
}
