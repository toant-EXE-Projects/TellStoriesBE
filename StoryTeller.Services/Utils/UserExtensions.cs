using Microsoft.AspNetCore.Identity;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;

namespace StoryTeller.Services.Utils
{
    public static class UserExtensions
    {
        public static async Task<bool> IsStaffAsync(this ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var roles = await userManager.GetRolesAsync(user);
            bool isStaff = roles.Intersect(Roles.Staff).Any();
            return isStaff;
        }
        public static async Task<bool> IsAdminAsync(this ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var roles = await userManager.GetRolesAsync(user);
            return roles.Contains(Roles.Admin);
        }
    }
}
