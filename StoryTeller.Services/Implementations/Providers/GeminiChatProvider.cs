using AutoMapper;
using Azure;
using Json.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mscc.GenerativeAI;
using StoryTeller.Data.Entities.AI;
using StoryTeller.Data.Logger;
using StoryTeller.Services.Interfaces.AI;
using StoryTeller.Services.Models.AI.OptionsModel;
using StoryTeller.Services.Models.AI.RequestModel;
using StoryTeller.Services.Models.AI.ResponseModel;
using StoryTeller.Services.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StoryTeller.Services.Implementations.Providers
{
    public class GeminiChatProvider : IChatProvider
    {
        private readonly string? _apiKey;
        private readonly IMapper _mapper;
        private readonly List<SafetySetting> _safetySettings;
        private readonly ILoggerService _logger;

        public GeminiChatProvider(IConfiguration config, IMapper mapper, ILoggerService logger)
        {
            _logger = logger;
            _mapper = mapper;
            _safetySettings = new List<SafetySetting>
            {
                new SafetySetting { Category = HarmCategory.HarmCategoryDangerousContent, Threshold = HarmBlockThreshold.BlockLowAndAbove },
                new SafetySetting { Category = HarmCategory.HarmCategoryHateSpeech, Threshold = HarmBlockThreshold.BlockLowAndAbove },
                new SafetySetting { Category = HarmCategory.HarmCategoryHarassment, Threshold = HarmBlockThreshold.BlockLowAndAbove },
                new SafetySetting { Category = HarmCategory.HarmCategorySexuallyExplicit, Threshold = HarmBlockThreshold.BlockLowAndAbove },
            };

            _apiKey = config["Gemini:ApiKey"];
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogWarning("⚠️ Gemini API key is missing. Service will be unavailable.");
            }
            //_logger.LogInfo($"[SERVICE API] Gemini: using API key {_apiKey}");
        }

        // https://ai.google.dev/gemini-api/docs/rate-limits#free-tier
        public async Task<string> GetResponseAsync(
            string prompt,
            IEnumerable<ChatMessageDTO> history,
            ChatGenerationOptions? options = null,
            CancellationToken ct = default)
        {
            var googleAI = new GoogleAI(apiKey: _apiKey);

            var systemInstructionText = string.Join(
                "\n",
                new[] { options?.SystemInstruction, options?.AdditionalSystemInstruction }
                    .Where(s => !string.IsNullOrWhiteSpace(s))
            );
            var systemInstruction = new Content(systemInstructionText);

            var genConfig = options?.ProviderConfig as GenerationConfig ?? new GenerationConfig();
            genConfig = _mapper.Map<GenerationConfig>(options);

            var model = googleAI.GenerativeModel(
                model: Model.Gemini25Flash,
                systemInstruction: systemInstruction
            );
            var priorMessages = history?
                .Where(h => !string.IsNullOrWhiteSpace(h.Content))
                .Select(m => new ContentResponse(m.Content, m.Role))
                .ToList();

            var chat = model.StartChat(priorMessages);

            var response = await chat.SendMessage(prompt,
                generationConfig: genConfig,
                safetySettings: _safetySettings,
                cancellationToken: ct
            );

            return response?.Text ?? string.Empty;
        }

        public async Task<string> GetStoryResponseAsync(
            string prompt,
            IEnumerable<ChatMessageDTO> history,
            ChatGenerationOptions? options = null,
            CancellationToken ct = default)
        {
            var googleAI = new GoogleAI(apiKey: _apiKey);

            var systemInstructionText = string.Join(
                "\n",
                new[] { options?.SystemInstruction, options?.AdditionalSystemInstruction }
                    .Where(s => !string.IsNullOrWhiteSpace(s))
            );
            var systemInstruction = new Content(systemInstructionText);

            var genConfig = options?.ProviderConfig as GenerationConfig ?? new GenerationConfig();
            genConfig = _mapper.Map<GenerationConfig>(options);

            genConfig.ResponseMimeType = "application/json";
            genConfig.ResponseSchema = new StoryGenerationResponse();

            var model = googleAI.GenerativeModel(
                model: Model.Gemini25Flash,
                systemInstruction: systemInstruction
            );

            var res = await model.GenerateContent(
                prompt,
                generationConfig: genConfig,
                safetySettings: _safetySettings,
                cancellationToken: ct
            );

            return res?.Text ?? string.Empty;
        }

        public async IAsyncEnumerable<string> StreamResponseAsync(
            string prompt,
            IEnumerable<ChatMessageDTO> history,
            ChatGenerationOptions? options = null,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            var googleAI = new GoogleAI(apiKey: _apiKey);

            var systemInstructionText = string.Join(
                "\n",
                new[] { options?.SystemInstruction, options?.AdditionalSystemInstruction }
                    .Where(s => !string.IsNullOrWhiteSpace(s))
            );
            var systemInstruction = new Content(systemInstructionText);

            var genConfig = options?.ProviderConfig as GenerationConfig ?? new GenerationConfig();
            genConfig = _mapper.Map<GenerationConfig>(options);

            var model = googleAI.GenerativeModel(
                model: Model.Gemini25Flash,
                systemInstruction: systemInstruction
            );
            var priorMessages = history?
                .Where(h => !string.IsNullOrWhiteSpace(h.Content))
                .Select(m => new ContentResponse(m.Content, m.Role))
                .ToList();

            var chat = model.StartChat(priorMessages);
            await foreach (var streamedResponse in chat.SendMessageStream(prompt, generationConfig: genConfig, safetySettings: _safetySettings, cancellationToken: ct))
            {
                if (!string.IsNullOrWhiteSpace(streamedResponse.Text))
                {
                    yield return streamedResponse.Text;
                }
            }
        }

        public async Task<List<object>> GenerateImagesAsync(AIImageGenerationRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                throw new ArgumentException("Prompt must not be empty.", nameof(request.Prompt));

            var googleAI = new GoogleAI(_apiKey);

            var model = googleAI.ImageGenerationModel(Model.Imagen4Preview);

            var imageRequest = new ImageGenerationRequest(request.Prompt, request.Count);
            try { 
                var response = await model.GenerateImages(imageRequest, ct);
                if (response?.Predictions == null || response.Predictions.Count == 0)
                    throw new InvalidOperationException("Image generation failed or returned no images.");

                return response.Predictions.Cast<object>().ToList();
            }
            catch (HttpRequestException ex)
            {
                var responseBody = ex.Message; // or log ex.Message
                throw new Exception($"Google AI image generation failed: {responseBody}", ex);
            }


        }

    }
}
