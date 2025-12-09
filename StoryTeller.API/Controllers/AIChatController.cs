using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.API.Utils;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Implementations.AI;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.AI.RequestModel;
using StoryTeller.Services.Models.AI.ResponseModel;
using StoryTeller.Services.Models.ResponseModel;
using System.Text.Json;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class AIChatController : ControllerBase
    {
        private readonly AIChatService _chatService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IUsageLimiter _usageLimiter;
        private readonly IUserContextService _userContext;

        public AIChatController(
            AIChatService chatService, 
            HttpClient httpClient, 
            IConfiguration config, 
            IUsageLimiter usageLimiter, 
            IUserContextService userContext
        )
        {
            _chatService = chatService;
            _httpClient = httpClient;
            _config = config;
            _usageLimiter = usageLimiter;
            _userContext = userContext;
        }

        [HttpPost("stream-ask")]
        [Authorize]
        public async Task StreamAsk([FromBody] AIChatRequest request, CancellationToken ct)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                await Response.WriteAsync("data: User not found\n\n", ct);
                return;
            }

            //-------------UsageCheck-------------//
            bool canProceed = await _usageLimiter.Check(
                user: user,
                httpContext: HttpContext,
                freeLimit: UsageLimitsConst.Free_AIAsk,
                windowHours: UsageLimitsConst.FreeCallsWindowHours,
                route: UsageLimitsConst.Route_AIAsk,
                ct: ct
            );

            if (!canProceed)
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                await Response.WriteAsync($"event: {StringConstants.vi_UsagelimitReached}\n\n{StringConstants.en_UsagelimitReached}", ct);
                await Response.Body.FlushAsync(ct);
                return;
            }
            //-------------UsageCheck-------------//

            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";

            await Response.WriteAsync(": connected\n\n", ct);
            await Response.Body.FlushAsync(ct);

            try
            {
                await foreach (var chunk in _chatService.StreamChatAsync(
                    request.Provider,
                    request.Prompt,
                    request.History,
                    request.Options,
                    ct
                ))
                {
                    if (!string.IsNullOrWhiteSpace(chunk))
                    {
                        var lines = chunk.Split('\n');
                        foreach (var line in lines)
                        {
                            await Response.WriteAsync($"data: {line}\n", ct);
                        }
                        await Response.WriteAsync("\n", ct);
                        await Response.Body.FlushAsync(ct);
                    }
                }
            }
            catch (Exception ex)
            {
                await Response.WriteAsync($"event: error\ndata: {ex.Message}\n\n", ct);
                await Response.Body.FlushAsync(ct);
            }
        }

        /// <summary>
        /// Send a prompt to the selected AI provider and get a generated response.
        /// </summary>
        /// <remarks>
        /// <b>Provider</b> MUST be one of the following:<br/>
        ///     "Gemini"
        /// <br/><br/>
        /// <b>Options</b> can be used to control the AI's generation style and output:
        /// <ul>
        ///   <li><b>Temperature</b>: (number, optional) Controls randomness. Lower is more deterministic, higher is more creative. Default: 0.6</li>
        ///   <li><b>TopP</b>: (number, optional) Nucleus sampling. Default: 0.8. Limits the AI to considering only the most likely next words whose combined probability exceeds this value (e.g., 0.8 means only the top 80% probable words are considered). Helps control diversity and randomness.</li>
        ///   <li><b>TopK</b>: (number, optional) Top-K sampling. Default: 20. Limits the AI to choosing the next word from only the top K most likely options (e.g., 20 means only the 20 most probable words are considered). Also helps control diversity and creativity.</li>
        ///   <li><b>StopSequences</b>: (array of string, optional) List of sequences where the AI will stop generating further text.</li>
        ///   <li><b>Seed</b>: (integer, optional) Random seed for reproducible results.</li>
        ///   <li><b>AdditionalSystemInstruction</b>: (string, optional) Extra instruction to guide the AI's behavior.</li>
        /// </ul>
        /// <br/>
        /// Sample:
        /// 
        ///     POST api/chat/Ask
        ///     {
        ///         "Provider": "Gemini",
        ///         "Prompt": "Tell me a bedtime story about a dragon and a princess.",
        ///         "Options": {
        ///             "Temperature": 0.6,
        ///             "TopP": 0.8,
        ///             "TopK": 20,
        ///             "StopSequences": [ "THE END" ],
        ///             "Seed": 12345,
        ///             "AdditionalSystemInstruction": "Always use polite language and keep the story suitable for children."
        ///         },
        ///         "history": [
        ///             {
        ///                 "role": "user",
        ///                 "content": "Hello! My name is sam!"
        ///             }
        ///         ]
        ///     }
        ///
        /// </remarks>
        /// <param name="request">The chat request containing provider, prompt, and generation options.</param>
        /// <param name="ct">Cancellation token for the request.</param>
        /// <returns>The AI-generated response as JSON.</returns>
        [HttpPost("ask")]
        [Authorize]
        public async Task<IActionResult> Ask([FromBody] AIChatRequest request, CancellationToken ct)
        {

            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            //-------------UsageCheck-------------//
            bool canProceed = await _usageLimiter.Check(
                user: user,
                httpContext: HttpContext,
                freeLimit: UsageLimitsConst.Free_AIAsk,
                windowHours: UsageLimitsConst.FreeCallsWindowHours,
                route: UsageLimitsConst.Route_AIAsk,
                ct: ct
            );

            if (!canProceed)
            {
                return StatusCode(403,
                    APIResponse<object>.ErrorResponse("Usage limit exceeded. Please upgrade your subscription.")
                );
            }
            //-------------UsageCheck-------------//

            var response = await _chatService.ChatAsync(request.Provider, request.Prompt, request.History, request.Options, ct: ct);

            if (string.IsNullOrWhiteSpace(response))
                return BadRequest(APIResponse<string>.ErrorResponse($"{request.Provider} returned Empty response"));

            return Ok(APIResponse<object>.SuccessResponse(response));
        }

        [HttpPost("generate-story")]
        [Authorize]
        public async Task<IActionResult> GenerateStory([FromBody] AIChatRequest request, CancellationToken ct)
        {

            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            //-------------UsageCheck-------------//
            bool canProceed = await _usageLimiter.Check(
                user: user,
                httpContext: HttpContext,
                freeLimit: UsageLimitsConst.Free_StoryGeneration,
                windowHours: UsageLimitsConst.FreeCallsWindowHours,
                route: UsageLimitsConst.Route_StoryGeneration,
                ct: ct
            );

            if (!canProceed)
            {
                return StatusCode(403,
                    APIResponse<object>.ErrorResponse("Usage limit exceeded. Please upgrade your subscription.")
                );
            }
            //-------------UsageCheck-------------//

            var generate = await _chatService.GenerateStoryAsync(request.Provider, request.Prompt, null, request.Options, ct: ct);
            if (string.IsNullOrWhiteSpace(generate))
            {
                return BadRequest(APIResponse<string>.ErrorResponse($"{request.Provider} returned Empty response"));
            }

            StoryGenerationResponse? response;
            try
            {
                response = JsonSerializer.Deserialize<StoryGenerationResponse>(generate, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                // Log or return an error if needed
                return StatusCode(500, APIResponse<string>.ErrorResponse($"JSON parse error: {ex.Message}"));
            }

            return Ok(APIResponse<StoryGenerationResponse>.SuccessResponse(response!));
        }

        //[HttpPost("generate-image")]
        //[Authorize]
        //public async Task<IActionResult> GenerateImagesAsync([FromBody] AIImageGenerationRequest request, CancellationToken ct)
        //{
        //    var user = await _userContext.GetCurrentUserAsync(User);
        //    if (user == null)
        //        return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

        //    //-------------UsageCheck-------------//
        //    //bool canProceed = await _usageLimiter.Check(
        //    //    user: user,
        //    //    httpContext: HttpContext,
        //    //    freeLimit: UsageLimitsConst.Free_ImageGeneration,
        //    //    windowHours: UsageLimitsConst.FreeCallsWindowHours,
        //    //    route: UsageLimitsConst.Route_ImageGeneration,
        //    //    ct: ct
        //    //);

        //    //if (!canProceed)
        //    //{
        //    //    return StatusCode(403,
        //    //        APIResponse<object>.ErrorResponse("Usage limit exceeded. Please upgrade your subscription.")
        //    //    );
        //    //}
        //    //-------------UsageCheck-------------//

        //    if (request.Count == 0)
        //        return BadRequest("\"[Count\"] Must be > 0");

        //    var images = await _chatService.GenerateImages(request.Provider, request, ct);

        //    if (images.Count == 0)
        //        return BadRequest("No images were generated.");

        //    return Ok(images); // You could also return image URLs or multipart
        //}

        //[HttpPost("history")]
        //[Authorize]
        //public async Task<IActionResult> GetChatHistory([FromBody] ChatHistoryFilter filter, CancellationToken ct)
        //{
        //    var history = await _chatService.GetChatHistoryAsync(filter, ct);
        //    return Ok(history);
        //}
    }
}
