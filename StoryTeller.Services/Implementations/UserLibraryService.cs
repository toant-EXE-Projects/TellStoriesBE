using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Utils;

namespace StoryTeller.Services.Implementations
{
    public class UserLibraryService : IUserLibraryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserLibraryService(IUnitOfWork uow, IConfiguration config, IMapper mapper, IDateTimeProvider dateTimeProvider, UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _config = config;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
            _userManager = userManager;
        }

        public async Task<List<UserLibraryDTO>> GetLibrariesByUserAsync(string userId)
        {
            var userLib = await _uow.UserLibraries.GetByUserIdAsync(userId);
            return _mapper.Map<List<UserLibraryDTO>>(userLib);
        }
        
        public async Task<List<UserLibraryMinimalDTO>> GetMinimalLibrariesByUserAsync(string userId)
        {
            var userLib = await _uow.UserLibraries.GetByUserIdAsync(userId);
            return _mapper.Map<List<UserLibraryMinimalDTO>>(userLib);
        }

        public async Task<List<UserLibraryDTO>> CreateDefaultLibrariesAsync(ApplicationUser user)
        {
            var userLibraries = await _uow.UserLibraries.GetByUserIdAsync(user.Id);

            var existingTitles = userLibraries
                .Where(l => l.IsSystemDefined)
                .Select(l => l.Title)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newLibraries = new List<UserLibrary>();

            foreach (var title in UserLibraryDefaults.DefaultCollections)
            {
                if (!existingTitles.Contains(title.Title))
                {
                    var lib = new UserLibrary
                    {
                        Title = title.Title,
                        UserId = user.Id,
                        Description = title.Description,
                        IsDeletable = false,
                        IsSystemDefined = true
                    };

                    await _uow.UserLibraries.CreateAsync(lib, user);
                    newLibraries.Add(lib);
                }
            }

            await _uow.SaveChangesAsync();

            return _mapper.Map<List<UserLibraryDTO>>(newLibraries);
        }

        public async Task<UserLibraryDTO> CreateLibraryAsync(UserLibraryCreateRequest request, ApplicationUser user)
        {
            var entity = _mapper.Map<UserLibrary>(request);
            entity.User = user;

            await _uow.UserLibraries.CreateAsync(entity, user);
            await _uow.SaveChangesAsync();

            return _mapper.Map<UserLibraryDTO>(entity);
        }

        public async Task<UserLibraryDTO?> GetLibraryCollectionAsync(Guid libraryId, ApplicationUser user)
        {
            var library = await _uow.UserLibraries
                .GetCollectionAsync(libraryId);

            if (library == null)
                return null;

            // Bypass check for admins
            if (await user.IsAdminAsync(_userManager))
            {
                return _mapper.Map<UserLibraryDTO>(library);
            }

            if (library.AccessType == Data.Enums.LibraryAccessType.Public)
                return _mapper.Map<UserLibraryDTO>(library);

            if (library.AccessType == Data.Enums.LibraryAccessType.Private)
            {
                if (library.UserId == user.Id)
                    return _mapper.Map<UserLibraryDTO>(library);

                return null;
            }

            return null;
        }

        public async Task<UserLibraryItemDTO> AddItemToLibraryAsync(UserLibraryItemCreateRequest request, ApplicationUser user)
        {
            var item = _mapper.Map<UserLibraryItem>(request);
            item.AddedAt = _dateTimeProvider.GetSystemCurrentTime();

            var libraryExists = await _uow.UserLibraries.GetByIdAsync(request.UserCollectionId);
            var storyExists = await _uow.Stories.GetByIdAsync(request.StoryId);
            if (libraryExists == null)
                throw new NotFoundException("Library Not Found.");
            if (storyExists == null || storyExists.IsDeleted)
                throw new NotFoundException("Story Not Found.");

            await _uow.UserLibraries.UpdateAsync(libraryExists, user); // Trigger UpdateAt/UpdateBy
            await _uow.UserLibraryItems.CreateAsync(item, user);
            await _uow.SaveChangesAsync();

            return _mapper.Map<UserLibraryItemDTO>(item);
        }

        public async Task<UserLibraryDTO> EditLibraryAsync(UserLibraryEditRequest request, ApplicationUser user)
        {
            var lib = await _uow.UserLibraries.GetByIdAsync(request.Id);
            if (lib == null)
                throw new NotFoundException("Library not found");

            if (lib.UserId != user.Id)
                throw new UnauthorizedAccessException("You do not own this library");

            lib = _mapper.Map(request, lib);

            await _uow.UserLibraries.UpdateAsync(lib, user);
            await _uow.SaveChangesAsync();

            return _mapper.Map<UserLibraryDTO>(lib);
        }

        public async Task<bool> DeleteLibraryAsync(Guid id, ApplicationUser user)
        {
            var lib = await _uow.UserLibraries.GetByIdAsync(id);
            if (lib == null)
                throw new NotFoundException("Library not found");

            if (lib.UserId != user.Id)
                throw new UnauthorizedAccessException("You do not own this library");
            if (!lib.IsDeletable)
                throw new UnauthorizedAccessException("You cannot delete this collection");
            if (lib.IsSystemDefined)
                throw new UnauthorizedAccessException("You cannot delete default collections");

            _uow.UserLibraries.Remove(lib);

            await _uow.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveLibraryItemAsync(Guid id, ApplicationUser user)
        {
            var item = await _uow.UserLibraryItems.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException("Item Not Found.");

            var libraryExists = await _uow.UserLibraries.GetByIdAsync(item.UserCollectionId);

            if (libraryExists == null)
                throw new NotFoundException("Library Not Found.");

            if (libraryExists.UserId != user.Id)
                throw new UnauthorizedAccessException("You do not own this library");

            await _uow.UserLibraries.UpdateAsync(libraryExists, user); // Trigger UpdateAt/UpdateBy
            _uow.UserLibraryItems.Remove(item);

            await _uow.SaveChangesAsync();

            return true;
        }
    }
}
