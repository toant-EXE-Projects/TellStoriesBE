using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StoryTeller.Services.Interfaces.AI;
using StoryTeller.Services.Models.AI.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Interfaces;
using StoryTeller.API.Utils;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class ImageGenerationController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IPollinationAIService _polliAIService;
        private readonly IUserContextService _userContext;
        private readonly IUsageLimiter _usageLimiter;

        public ImageGenerationController(IPollinationAIService polliAIService, 
            HttpClient httpClient, 
            IConfiguration config, 
            IUserContextService userContext,
            IUsageLimiter usageLimiter
        )
        {
            _httpClient = httpClient;
            _config = config;
            _polliAIService = polliAIService;
            _userContext = userContext;
            _usageLimiter = usageLimiter;
        }

        /// <summary>
        /// Generates an image based on a text prompt using the Pollination AI service.
        /// </summary>
        /// <param name="request">
        /// The image generation request containing:
        /// <br/> - <c>Prompt</c> (Optional): A text description of the image to generate (required).
        /// <br/> - <c>Width</c> (Optional): Width of the image in pixels. Default is 512.
        /// <br/> - <c>Height</c> (Optional): Height of the image in pixels. Default is 512.
        /// <br/> - <c>ModelId</c> (Optional): The model to use. Options: <c>flux</c>, <c>kontext</c>, <c>turbo</c>, <c>gptimage</c>. Default is <c>flux</c>.
        /// </param>
        /// <param name="ct">Optional cancellation token for request cancellation.</param>
        /// <returns>
        /// A PNG image file representing the generated image.
        /// </returns>
        [HttpPost("polinationai/generate-image")]
        [Authorize]
        public async Task<IActionResult> Polination_GenerateImage(PolinationImageGenerationRequest request, CancellationToken ct)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            //-------------UsageCheck-------------//
            bool canProceed = await _usageLimiter.Check(
                user: user,
                httpContext: HttpContext,
                freeLimit: UsageLimitsConst.Free_ImageGeneration,
                windowHours: UsageLimitsConst.FreeCallsWindowHours,
                route: UsageLimitsConst.Route_ImageGeneration,
                ct: ct
            );

            if (!canProceed)
            {
                return StatusCode(403,
                    APIResponse<object>.ErrorResponse("Usage limit exceeded. Please upgrade your subscription.")
                );
            }
            //-------------UsageCheck-------------//

            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest(APIResponse<object>.ErrorResponse("Prompt is required."));

            try
            {
                var imageBytes = await _polliAIService.GenerateImageAsync(request, ct);
                return File(imageBytes, "image/png");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, APIResponse<object>.ErrorResponse("Pollinations API error: " + ex.Message));
            }
        }

    }
}
