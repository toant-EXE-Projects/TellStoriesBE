using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Data.Logger;
using StoryTeller.Services.Interfaces;

namespace StoryTeller.Services.Implementations
{
    public class MigrationService : IMigrationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerService _logger;
        private readonly IUserInitializationService _userInitService;
        private readonly IStoryService _storyService;

        public MigrationService(
            IUnitOfWork uow, 
            IConfiguration config, 
            IMapper mapper, 
            IDateTimeProvider dateTimeProvider, 
            UserManager<ApplicationUser> userManager,
            ILoggerService loggerService,
            IUserInitializationService userInitService,
            IStoryService storyService
        )
        {
            _uow = uow;
            _config = config;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
            _userManager = userManager;
            _logger = loggerService;
            _userInitService = userInitService;
            _storyService = storyService;
        }

        public async Task<int> MigrateAllUsersAsync(CancellationToken ct = default)
        {
            var users = await _userManager.Users.ToListAsync(ct);
            int updatedCount = 0;

            foreach (var user in users)
            {
                await _userInitService.InitializeAsync(user.Id, ct);
                updatedCount++;
            }

            return updatedCount;
        }

        public async Task<bool> MigrateUserAsync(string userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            await _userInitService.InitializeAsync(userId, ct);
            return true;
        }


        public async Task<int> MigrateStoryTypesAsync(int type, CancellationToken cancellationToken = default)
        {
            var stories = await _uow.Stories.GetStoryWithDetailsQuery().ToListAsync();

            if (type == 1)
            {
                foreach (var story in stories)
                {
                    story.StoryType = story.Panels != null && story.Panels.Count > 1
                        ? eStoryType.MultiPanel
                        : eStoryType.SinglePanel;
                }
            }
            else if (type == 2)
            {
                foreach (var story in stories)
                {
                    bool hasImage = story.Panels != null
                        && story.Panels.Any(
                            panel => !string.IsNullOrWhiteSpace(panel.ImageUrl)
                        );

                    story.StoryType = hasImage
                        ? eStoryType.Illustrated
                        : eStoryType.Narrative;
                }
            }
            else { return 0; }


            return await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
