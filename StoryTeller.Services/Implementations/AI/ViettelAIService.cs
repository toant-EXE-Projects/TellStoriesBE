using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StoryTeller.Data.Logger;
using StoryTeller.Data.Utils;
using StoryTeller.Services.Interfaces.AI;
using StoryTeller.Services.Models.AI.RequestModel;
using StoryTeller.Services.Utils;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace StoryTeller.Services.Implementations.AI
{
    public class ViettelAIService : IViettelAIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILoggerService _logger;
        private readonly string? _apiKey;

        public ViettelAIService(IHttpClientFactory httpClientFactory, IConfiguration config, ILoggerService loggerService)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = loggerService;

            _apiKey = _config["ViettelAI:ApiKey"];
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogWarning("⚠️ ViettelAI API key is missing. Service will be unavailable.");
            }
            //_logger.LogInfo($"[SERVICE API] ViettelAI: using API key {_apiKey}");
        }

        public async Task<byte[]> GenerateAndMergeSpeechAsync(ViettelTTSRequest request, CancellationToken ct = default)
        {
            var chunks = StringUtils.SplitTextIntoChunks(request.Text, request.ChunkCharacterLength);
            var audioChunks = new List<byte[]>();

            foreach (var chunk in chunks)
            {
                var subRequest = new ViettelTTSRequest
                {
                    Text = chunk,
                    VoiceID = request.VoiceID,
                    Speed = request.Speed,
                    ReturnOption = request.ReturnOption,
                    WithoutFilter = request.WithoutFilter
                };

                var audio = await GenerateSpeechAsync(subRequest, ct);
                audioChunks.Add(audio);
            }

            return await new AudioHelper().MergeMp3Async(audioChunks, request.ChunkDelayms);
        }

        //https://viettelai.vn/tai-lieu
        public async Task<byte[]> GenerateSpeechAsync(ViettelTTSRequest request, CancellationToken ct = default)
        {
            var token = _config["ViettelAI:ApiKey"];
            var voice = request.VoiceID ?? "hcm-diemmy";
            var speed = request.Speed ?? 1.0;
            var returnOption = request.ReturnOption ?? 3;
            var withoutFilter = request.WithoutFilter;
            //_logger.LogInfo($"Using Viettel API key: {token}");
            var payload = new
            {
                text = request.Text,
                voice = voice,
                speed = speed,
                token = token,
                tts_return_option = returnOption,
                without_filter = withoutFilter
            };

            var client = _httpClientFactory.CreateClient();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://viettelai.vn/tts/speech_synthesis")
            {
                Content = JsonContent.Create(payload)
            };
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException("Failed to call Viettel TTS API: " + error);
            }

            var audioBytes = await response.Content.ReadAsByteArrayAsync();

            return audioBytes;
        }
    }
}
