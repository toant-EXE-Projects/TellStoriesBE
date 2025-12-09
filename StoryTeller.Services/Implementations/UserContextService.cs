using Microsoft.AspNetCore.Identity;
using StoryTeller.Data.Entities;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using System.Security.Claims;

namespace StoryTeller.Services.Interfaces
{
    public class UserContextService : IUserContextService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserContextService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return null;

            return await _userManager.FindByIdAsync(userId);
        }
    }
}
