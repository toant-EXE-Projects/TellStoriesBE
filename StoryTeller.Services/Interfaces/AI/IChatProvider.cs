using StoryTeller.Data.Entities.AI;
using StoryTeller.Services.Models.AI.OptionsModel;
using StoryTeller.Services.Models.AI.RequestModel;
using StoryTeller.Services.Models.DTO;

namespace StoryTeller.Services.Interfaces.AI
{
    public interface IChatProvider
    {
        Task<string> GetResponseAsync(string prompt, IEnumerable<ChatMessageDTO> history, ChatGenerationOptions? options = null, CancellationToken ct = default);
        Task<string> GetStoryResponseAsync(string prompt, IEnumerable<ChatMessageDTO> history, ChatGenerationOptions? options = null, CancellationToken ct = default);
        IAsyncEnumerable<string> StreamResponseAsync(string prompt, IEnumerable<ChatMessageDTO> history, ChatGenerationOptions? options = null, CancellationToken ct = default);
        Task<List<object>> GenerateImagesAsync(AIImageGenerationRequest request, CancellationToken ct = default);
    }
}
