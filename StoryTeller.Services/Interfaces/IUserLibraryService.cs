using StoryTeller.Data.Entities;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.Services.Interfaces
{
    public interface IUserLibraryService
    {
        Task<List<UserLibraryDTO>> GetLibrariesByUserAsync(string userId);
        Task<List<UserLibraryMinimalDTO>> GetMinimalLibrariesByUserAsync(string userId);
        Task<UserLibraryDTO> CreateLibraryAsync(UserLibraryCreateRequest request, ApplicationUser user);
        Task<UserLibraryDTO?> GetLibraryCollectionAsync(Guid libraryId, ApplicationUser user);
        Task<UserLibraryItemDTO> AddItemToLibraryAsync(UserLibraryItemCreateRequest request, ApplicationUser user);
        Task<bool> DeleteLibraryAsync(Guid id, ApplicationUser user);
        Task<bool> RemoveLibraryItemAsync(Guid id, ApplicationUser user);
        Task<List<UserLibraryDTO>> CreateDefaultLibrariesAsync(ApplicationUser user);
        Task<UserLibraryDTO> EditLibraryAsync(UserLibraryEditRequest request, ApplicationUser user);
    }
}
