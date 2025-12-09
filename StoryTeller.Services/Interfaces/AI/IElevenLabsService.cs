using ElevenLabs;
using StoryTeller.Data.Entities.AI;
using StoryTeller.Services.Models.AI;
using StoryTeller.Services.Models.AI.RequestModel;
using StoryTeller.Services.Models.RequestModel;

namespace StoryTeller.Services.Interfaces.AI
{
    public interface IElevenLabsService
    {
        Task<byte[]> GenerateAndMergeSpeechAsync(ElevenLabsTTSRequest request, CancellationToken ct = default);
        Task<byte[]> GenerateSpeechAsync(ElevenLabsTTSRequest request, CancellationToken ct = default);
        Task<GetVoicesResponseModel> GetVoiceList();
    }
}
