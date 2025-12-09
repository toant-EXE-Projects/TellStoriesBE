using ElevenLabs;
using StoryTeller.Data.Entities.AI;
using StoryTeller.Services.Models.AI;
using StoryTeller.Services.Models.AI.RequestModel;
using StoryTeller.Services.Models.RequestModel;

namespace StoryTeller.Services.Interfaces.AI
{
    public interface IPollinationAIService
    {
        Task<byte[]> GenerateImageAsync(PolinationImageGenerationRequest request, CancellationToken ct = default);
        Task<byte[]> GenerateSpeechAsync(PollinationTTSRequest request, CancellationToken ct = default);
        public Task<byte[]> GenerateAndMergeSpeechAsync(PollinationTTSRequest request, CancellationToken ct = default);
    }
}
