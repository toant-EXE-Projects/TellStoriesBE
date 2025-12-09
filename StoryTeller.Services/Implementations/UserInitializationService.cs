using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Logger;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Utils;

namespace StoryTeller.Services.Implementations
{
    public class UserInitializationService : IUserInitializationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserLibraryService _userLibraryService;
        private readonly ILoggerService _logger;

        public UserInitializationService(
            IUnitOfWork uow, 
            IConfiguration config, 
            IMapper mapper, 
            IDateTimeProvider dateTimeProvider, 
            UserManager<ApplicationUser> userManager,
            IUserLibraryService userLibraryService,
            ILoggerService loggerService
        )
        {
            _uow = uow;
            _config = config;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
            _userManager = userManager;
            _userLibraryService = userLibraryService;
            _logger = loggerService;
        }

        public async Task<bool> InitializeAsync(string userId, CancellationToken ct = default)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"Initialization skipped: User with ID {userId} not found.");
                    return false;
                }
                await _userLibraryService.CreateDefaultLibrariesAsync(user);
                return true;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"Initialization for UserId: {userId} was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError( $"Error initializing user data for UserId: {userId}", ex);
                throw;
            }
        }
    }
}
