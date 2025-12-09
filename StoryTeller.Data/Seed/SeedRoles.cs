using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;

namespace StoryTeller.Data.Seed
{
    public static class SeedRoles
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dateTimeProvider = serviceProvider.GetRequiredService<IDateTimeProvider>();

            foreach (var role in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            //----------------------------------------------------------------//
            var adminEmail = "admin@storyteller.com";
            var adminUserName = "admin@storyteller.com";
            var adminDisplayName = "Super Admin";
            var adminUserType = Roles.Admin;
            var adminPassword = "Admin123!";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    DisplayName = adminDisplayName,
                    UserType = adminUserType,
                    CreatedDate = dateTimeProvider.GetSystemCurrentTime(),
                    Status = UserStatusConstants.Active
                };

                var result = await userManager.CreateAsync(user, adminPassword); // Secure password in real app
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, adminUserType);
            }
            //----------------------------------------------------------------//

        }
    }

}
