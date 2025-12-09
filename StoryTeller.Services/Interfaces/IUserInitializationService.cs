using StoryTeller.Data.Entities;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.Services.Interfaces
{
    public interface IUserInitializationService
    {
        public Task<bool> InitializeAsync(string userId, CancellationToken ct = default);
    }
}
