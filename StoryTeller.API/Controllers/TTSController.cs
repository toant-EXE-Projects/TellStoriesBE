using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.API.Utils;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Interfaces.AI;
using StoryTeller.Services.Models.AI.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class TTSController : ControllerBase
    {
        private readonly IElevenLabsService _elevenLabsService;
        private readonly IViettelAIService _ViettelAIService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPollinationAIService _pollinationAIService;
        private readonly IUserContextService _userContext;
        private readonly IUsageLimiter _usageLimiter;

        public TTSController(IDateTimeProvider dateTimeProvider,
            IElevenLabsService elevenLabsService,
            IViettelAIService viettelAIService,
            IPollinationAIService pollinationAIService,
            IUserContextService userContext,
            IUsageLimiter usageLimiter
            )
        {
            _elevenLabsService = elevenLabsService;
            _ViettelAIService = viettelAIService;
            _dateTimeProvider = dateTimeProvider;
            _pollinationAIService = pollinationAIService;
            _userContext = userContext;
            _usageLimiter = usageLimiter;
        }

        private string GenerateSpeechFileName() => 
            $"Speech_{_dateTimeProvider.GetSystemCurrentTime()}_{Guid.NewGuid().ToString().Substring(0, 6)}";
        
        [HttpPost("elevenlabs/generate-tts")]
        [Authorize]
        public async Task<IActionResult> ElevenLabs_GenerateTTS(ElevenLabsTTSRequest request, CancellationToken ct)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            //-------------UsageCheck-------------//
            bool canProceed = await _usageLimiter.Check(
                user: user,
                httpContext: HttpContext,
                freeLimit: UsageLimitsConst.Free_TTSGeneration,
                windowHours: UsageLimitsConst.FreeCallsWindowHours,
                route: UsageLimitsConst.Route_TTSGeneration,
                ct: ct
            );

            if (!canProceed)
            {
                return StatusCode(403,
                    APIResponse<object>.ErrorResponse("Usage limit exceeded. Please upgrade your subscription.")
                );
            }
            //-------------UsageCheck-------------//

            try
            {
                byte[] audioBytes;
                if (request.AutoChunk == true)
                {
                    audioBytes = await _elevenLabsService.GenerateAndMergeSpeechAsync(request, ct);
                }
                else
                {
                    audioBytes = await _elevenLabsService.GenerateSpeechAsync(request, ct);
                }
                var fileName = GenerateSpeechFileName();

                return File(audioBytes, "audio/mpeg", $"{fileName}.mp3");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, APIResponse<string>.ErrorResponse($"ElevenLabs TTS service error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("elevenlabs/voices")]
        [Authorize]
        public async Task<IActionResult> ElevenLabs_VoiceList()
        {
            var voices = await _elevenLabsService.GetVoiceList();

            //var voices = ElevenLabsVoices.GetAllVoices();
            return Ok(APIResponse<object>.SuccessResponse(data: voices));
        }

        /// <summary>
        /// Generates speech audio from text using the Viettel AI TTS service.
        /// </summary>
        /// <param name="request">
        /// The TTS request payload. Includes:
        /// - <c>Text</c>: The input text to synthesize.
        /// - <c>VoiceID</c> (optional): The voice ID to use. Default is "hcm-diemmy".
        ///   * hcm-diemmy
        ///   * hcm-minhquan
        ///   * hcm-phuongly
        ///   * hcm-thuydung
        ///   * hcm-thuyduyen
        ///   * hn-leyen
        ///   * hn-namkhanh
        ///   * hn-phuongtrang
        ///   * hn-quynhanh
        ///   * hn-thanhha
        ///   * hn-thanhphuong
        ///   * hn-thanhtung
        ///   * hn-thaochi
        ///   * hn-tienquan
        ///   * hue-baoquoc
        ///   * hue-maingoc
        /// - <c>Speed</c> (optional): Speaking speed, from 0.5 (slow) to 2.0 (fast). Default is 1.0.
        /// - <c>WithoutFilter</c> (optional): If true, disables sensitive content filtering. Default is false.
        /// </param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>
        /// Returns an audio stream (MP3 or WAV) containing the synthesized speech.
        /// </returns>
        [HttpPost("viettelAI/generate-tts")]
        [Authorize]
        public async Task<IActionResult> ViettelAI_GenerateTTS(ViettelTTSRequest request, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            //-------------UsageCheck-------------//
            bool canProceed = await _usageLimiter.Check(
                user: user,
                httpContext: HttpContext,
                freeLimit: UsageLimitsConst.Free_TTSGeneration,
                windowHours: UsageLimitsConst.FreeCallsWindowHours,
                route: UsageLimitsConst.Route_TTSGeneration,
                ct: ct
            );

            if (!canProceed)
            {
                return StatusCode(403,
                    APIResponse<object>.ErrorResponse("Usage limit exceeded. Please upgrade your subscription.")
                );
            }
            //-------------UsageCheck-------------//
            try
            {
                byte[] audioBytes;
                if (request.AutoChunk == true)
                {
                    audioBytes = await _ViettelAIService.GenerateAndMergeSpeechAsync(request, ct);
                }
                else
                {
                    audioBytes = await _ViettelAIService.GenerateSpeechAsync(request, ct);
                }
                var fileName = GenerateSpeechFileName();

                return File(audioBytes, "audio/mpeg", $"{fileName}.mp3");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, APIResponse<string>.ErrorResponse($"Viettel TTS service error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.ErrorResponse($"Viettel TTS service error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Generates speech audio from text using the Pollination AI TTS service.
        /// </summary>
        /// <param name="request">
        /// The TTS request payload containing:
        /// - <c>Prompt</c>: The input text to synthesize.
        /// - <c>VoiceId</c> (Optional): The voice ID to use. Default is "alloy".
        ///   * alloy
        ///   * ash
        ///   * coral
        ///   * echo
        ///   * fable
        ///   * onyx
        ///   * nova
        ///   * sage
        ///   * shimmer
        ///   * verse
        /// - <c>AdditionalInstructions</c> (Optional): Additional system instructions (https://www.openai.fm/)
        /// </param>
        /// <param name="ct">Optional cancellation token for request cancellation.</param>
        /// <returns>
        /// An MP3 audio stream representing the synthesized speech.
        /// </returns>
        /// <exception cref="HttpRequestException">Thrown when the Pollination TTS service is unreachable or returns an error.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
        [HttpPost("pollinationai/generate-tts")]
        [Authorize]
        public async Task<IActionResult> PollinationAI_GenerateTTS(PollinationTTSRequest request, CancellationToken ct = default)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            //-------------UsageCheck-------------//
            bool canProceed = await _usageLimiter.Check(
                user: user,
                httpContext: HttpContext,
                freeLimit: UsageLimitsConst.Free_TTSGeneration,
                windowHours: UsageLimitsConst.FreeCallsWindowHours,
                route: UsageLimitsConst.Route_TTSGeneration,
                ct: ct
            );

            if (!canProceed)
            {
                return StatusCode(403,
                    APIResponse<object>.ErrorResponse("Usage limit exceeded. Please upgrade your subscription.")
                );
            }
            //-------------UsageCheck-------------//
            try
            {
                byte[] audioBytes;
                if (request.AutoChunk == true)
                {
                    audioBytes = await _pollinationAIService.GenerateAndMergeSpeechAsync(request, ct);
                }
                else
                {
                    audioBytes = await _pollinationAIService.GenerateSpeechAsync(request, ct);
                }

                var fileName = GenerateSpeechFileName();

                return File(audioBytes, "audio/mpeg", $"{fileName}.mp3");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, APIResponse<string>.ErrorResponse($"Pollination TTS service error: {ex.Message}"));
            }
            catch (OperationCanceledException ex)
            {
                return StatusCode(499, APIResponse<string>.ErrorResponse($"Cancelled: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.ErrorResponse($"Pollination TTS service error: {ex.Message}"));
            }
        }
    }
}
