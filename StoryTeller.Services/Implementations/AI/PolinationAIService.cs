using Microsoft.Extensions.Configuration;
using NAudio.Lame;
using NAudio.Wave;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Logger;
using StoryTeller.Data.Utils;
using StoryTeller.Services.Interfaces.AI;
using StoryTeller.Services.Models.AI.RequestModel;
using StoryTeller.Services.Utils;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace StoryTeller.Services.Implementations.AI
{

    public class PollinationAIService : IPollinationAIService
    {

        private readonly IConfiguration _config;
        private readonly string? _apiKey;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _polliDomain = "pollinations.ai";
        private readonly ILoggerService _logger;

        public PollinationAIService(IConfiguration config, IHttpClientFactory httpClientFactory, ILoggerService logger)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            _apiKey = _config["PollinationAI:ApiKey"];
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                logger.LogWarning("⚠️ PollinationAI API key is missing. Service will be disabled.");
            }
            //_logger.LogInfo($"[SERVICE API] PollinationAI: using API key {_apiKey}");
        }

        // https://github.com/pollinations/pollinations/blob/master/APIDOCS.md#generate-image-api
        public async Task<byte[]> GenerateImageAsync(PolinationImageGenerationRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                throw new ArgumentException("Prompt is required.", nameof(request.Prompt));

            var encodedPrompt = Uri.EscapeDataString(request.Prompt);

            var baseUrl = $"https://image.{_polliDomain}/prompt/{encodedPrompt}";

            var queryParams = new Dictionary<string, string>
            {
                ["model"] = request.ModelId!,
                ["width"] = request.Width.ToString()!,
                ["height"] = request.Height.ToString()!,
                ["nologo"] = "true",
                ["enhance"] = "true",
                ["private"] = "true",
                ["safe"] = "true"
            };

            if (request.Seed.HasValue)
            {
                queryParams["seed"] = request.Seed.Value.ToString();
            }

            var url = QueryHelpers.AddQueryString(baseUrl, queryParams);
            //_logger.LogInfo(url);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(httpRequest, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Pollinations API failed ({(int)response.StatusCode}): {errorContent}");
            }

            _logger.LogInfo($"Created Image with prompt: {request.Prompt} | Model: {request.ModelId}");

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> GenerateAndMergeSpeechAsync(PollinationTTSRequest request, CancellationToken ct = default)
        {
            var chunks = StringUtils.SplitTextIntoChunks(request.Text, request.ChunkCharacterLength);
            var audioChunks = new List<byte[]>();

            foreach (var chunk in chunks)
            {
                var subRequest = new PollinationTTSRequest
                {
                    Text = chunk,
                    VoiceId = request.VoiceId,
                    AdditionalInstructions = request.AdditionalInstructions
                };
                var audio = await GenerateSpeechAsync(subRequest, ct);
                audioChunks.Add(audio);
            }

            return await new AudioHelper().MergeMp3Async(audioChunks, request.ChunkDelayms);
        }

        //https://github.com/pollinations/pollinations/blob/master/APIDOCS.md#audio-generation-api
        //https://www.openai.fm/
        public async Task<byte[]> GenerateSpeechAsync(PollinationTTSRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                throw new ArgumentException("Text is required.", nameof(request.Text));

            var instruction = string.Format(AISystemInstruction.PolinationAI_TTS, request.AdditionalInstructions);
                
            var body = new
            {
                model = "openai-audio",
                modalities = new[] { "text", "audio" },
                audio = new
                {
                    voice = request.VoiceId,
                    format = "mp3"
                },
                messages = new[]
                {
                    new { role = "system", content = instruction },
                    new { role = "user", content = request.Text }
                },
                @private = true,
            };

            var url = $"https://text.{_polliDomain}/openai";

            var json = JsonSerializer.Serialize(body);
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Headers = {
                    Authorization = new AuthenticationHeaderValue("Bearer", _apiKey)
                }
            };

            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(httpRequest, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                throw new HttpRequestException($"Pollinations TTS API error ({(int)response.StatusCode}): {error}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync(ct);

            // Deserialize and extract base64 audio
            using var doc = JsonDocument.Parse(jsonResponse);
            var base64 = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("audio")
                .GetProperty("data")
                .GetString();

            if (string.IsNullOrWhiteSpace(base64))
                throw new InvalidOperationException("No audio data returned from Pollinations.");

            return Convert.FromBase64String(base64);
        }
    }
}
