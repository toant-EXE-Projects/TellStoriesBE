using ElevenLabs;
using Microsoft.Extensions.Configuration;
using StoryTeller.Data.Logger;
using StoryTeller.Data.Utils;
using StoryTeller.Services.Interfaces.AI;
using StoryTeller.Services.Models.AI.RequestModel;
using StoryTeller.Services.Utils;

namespace StoryTeller.Services.Implementations.AI
{

    public class ElevenLabsService : IElevenLabsService
    {

        private readonly IConfiguration _config;
        private readonly IKeyRotator _elevenLabsRotator;
        private readonly ILoggerService _logger;

        public ElevenLabsService(
            IConfiguration config,
            IDictionary<string, IKeyRotator> elevenLabsRotator,
            ILoggerService logger
        )
        {
            _config = config;
            _logger = logger;

            if (!elevenLabsRotator.TryGetValue("ElevenLabs", out _elevenLabsRotator))
            {
                _logger.LogWarning("⚠️ Warning: No key rotator configured for ElevenLabs. Service will be unavailable.");
                _elevenLabsRotator = null!;
            }
            //_logger.LogInfo($"[SERVICE API] ElevenLabs: using API key {_elevenLabsRotator.GetNextKey()}");
        }

        public async Task<GetVoicesResponseModel> GetVoiceList()
        {
            string apiKey = _elevenLabsRotator.GetNextKey();

            var api = new ElevenLabsClient(apiKey);

            return await api.Voices.GetVoicesAsync();
        }

        public async Task<byte[]> GenerateAndMergeSpeechAsync(ElevenLabsTTSRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                throw new ArgumentException("Text must not be empty.", nameof(request.Text));

            var chunks = StringUtils.SplitTextIntoChunks(request.Text, request.ChunkCharacterLength);
            var audioChunks = new List<byte[]>();

            foreach (var chunk in chunks)
            {
                var subRequest = new ElevenLabsTTSRequest
                {
                    Text = chunk,
                    VoiceId = request.VoiceId,
                    Speed = request.Speed,
                    ModelId = request.ModelId,
                    LanguageCode = request.LanguageCode,
                    ElevenLabsAPIKey = request.ElevenLabsAPIKey,
                    ChunkCharacterLength = request.ChunkCharacterLength,
                    ChunkDelayms = request.ChunkDelayms
                };

                var audio = await GenerateSpeechAsync(subRequest, ct);
                audioChunks.Add(audio);
            }

            return await new AudioHelper().MergeMp3Async(audioChunks, request.ChunkDelayms);
        }
        public async Task<byte[]> GenerateSpeechAsync(ElevenLabsTTSRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                throw new ArgumentException("Text must not be empty.", nameof(request.Text));
            string apiKey;
            if (request.ElevenLabsAPIKey == null)
                apiKey = _elevenLabsRotator.GetNextKey();
            else
                apiKey = request.ElevenLabsAPIKey;
            
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("Elevenlabs API key missing.");

            //Console.WriteLine(apiKey);
            var api = new ElevenLabsClient(apiKey);

            var VOsettings = new VoiceSettingsResponseModel
            {
                Stability = 0.5,
                SimilarityBoost = 0.75,
                Speed = request.Speed
            };

            var voice = await api.TextToSpeech.CreateTextToSpeechByVoiceIdAsync(
                modelId: request.ModelId,
                languageCode: request.LanguageCode,
                text: request.Text,
                voiceId: request.VoiceId,
                voiceSettings: VOsettings,
                cancellationToken: ct
            );
            return voice;
        }
    }
}
