using HotChocolate.Authorization;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    [Authorize(Policy = Policies.StaffOnly)]
    public class UserQuery
    {
        public async Task<List<UserDTO>> GetUsers([Service] IUserService userService)
            => await userService.GetAllAsync();

        public async Task<UserDTO?> GetUserByEmail(string email, [Service] IUserService userService)
            => await userService.GetByEmailAsync(email);

        public async Task<UserDTO?> GetUserById(string id, [Service] IUserService userService)
            => await userService.GetByIdAsync(id);

        public async Task<List<UserDTO>> SearchByNameAndEmail(string searchValue, [Service] IUserService userService)
            => await userService.SearchAsync(searchValue);

    }
}
